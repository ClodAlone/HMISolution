#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
//
#endregion

using System;

namespace Syncfusion.Pdf.Compression
{
    /// <summary>
    /// PdfPngFilter provides methods that allows to restore original data
    /// being modified by similar filter or modify data to better compression ratio.
    /// </summary>
    internal class PdfPngFilter
    {
        #region Internals

        /// <summary>
        /// Defines Png Filtern Type.
        /// </summary>
        internal enum Type
        {
            /// <summary>
            ///  None compression.
            /// </summary>
            None = 0,

            /// <summary>
            /// Sub compression
            /// </summary>
            Sub = 1,

            /// <summary>
            /// Up compression
            /// </summary>
            Up = 2,

            /// <summary>
            /// Average compression
            /// </summary>
            Average = 3,

            /// <summary>
            /// Paeth compression
            /// </summary>
            Paeth = 4
        }
        #endregion

        #region Static fields
        /// <summary>
        /// Delegate for the sub filter.
        /// </summary>
        private static RowFilter s_subFilter = new RowFilter(CompressSub);

        /// <summary>
        /// Delegate for the up filter.
        /// </summary>
        private static RowFilter s_upFilter = new RowFilter(CompressUp);

        /// <summary>
        /// Delegate for the average filter.
        /// </summary>
        private static RowFilter s_averageFilter = new RowFilter(CompressAverage);

        /// <summary>
        /// Delegate for the Paeth filter.
        /// </summary>
        private static RowFilter s_paethFilter = new RowFilter(CompressPaeth);

        /// <summary>
        /// Delegate for the restore filter.
        /// </summary>
        private static RowFilter s_decompressFilter = new RowFilter(Decompress);

        /// <summary>
        /// Required for type cast.
        /// </summary>
        private const byte m_zero = 0;
        #endregion

        #region Static public methods
        /// <summary>
        /// Modifies the data by the filter of type 'type'.
        /// </summary>
        /// <param name="data">The data to modify.</param>
        /// <param name="bpr">Bytes per row.</param>
        /// <param name="type">The type of the filter.</param>
        /// <returns>The modified data.</returns>
        public static byte[] Compress(byte[] data, int bpr, Type type)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (bpr <= 0)
            {
                throw new ArgumentException("There can't be less or equal to zero bytes in a line.", "bpr");
            }

            RowFilter rf;

            switch (type)
            {
                case Type.None:
                    return data;

                case Type.Sub:
                    rf = s_subFilter;
                    break;

                case Type.Up:
                    rf = s_upFilter;
                    break;

                case Type.Average:
                    rf = s_averageFilter;
                    break;

                case Type.Paeth:
                    rf = s_paethFilter;
                    break;

                default:
                    throw new ArgumentException("Unsupported PNG filter: " + type.ToString(),
                        "type");
            }

            return Modify(data, bpr, rf, true);
        }

        /// <summary>
        /// Retrieves the original data from the modified.
        /// </summary>
        /// <param name="data">The modified data.</param>
        /// <param name="bpr">Bytes per row.</param>
        /// <returns>The original data.</returns>
        public static byte[] Decompress(byte[] data, int bpr)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (bpr <= 0)
            {
                throw new ArgumentException("There can't be less or equal to zero bytes in a line.", "bpr");
            }
            return Modify(data, bpr + 1, s_decompressFilter, false);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Performs actual compression or decompression.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="bpr">The BPR.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="pack">if it is pack, set to <c>true</c></param>
        /// <returns></returns>
        private static byte[] Modify(byte[] data, int bpr, RowFilter filter, bool pack)
        {
            long index = 0;
            long length = data.Length;
            long items = length / bpr;
            int outBPR = bpr - ((pack) ? -1 : 1);
            long outLength = (pack) ? items * outBPR : items * outBPR;

            byte[] result = new byte[outLength];
            int currentRow = 0;

            while (index + bpr <= length)
            {
                filter(data, index, bpr, result, currentRow, outBPR);

                currentRow += outBPR;
                index += bpr;
            }

            return result;
        }

        /// <summary>
        /// Modifies a row by the Sub algorithm.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void CompressSub(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            result[resIndex] = 1; // Set the filter type.
            ++resIndex;

            for (int i = 0; i < resBPR; ++i)
            {
                result[resIndex] = (byte)(data[inIndex]
                    - ((i > 0) ? (data[inIndex - 1]) : (m_zero)));

                ++resIndex;
                ++inIndex;
            }
        }

        /// <summary>
        /// Modifies a row by the Up algorithm.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void CompressUp(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            long prevIndex = inIndex - inBPR;

            result[resIndex] = 2; // Set the filter type.
            ++resIndex;

            for (int i = 0; i < inBPR; ++i)
            {
                result[resIndex] = (byte)(data[inIndex] -
                    ((prevIndex < 0) ? (m_zero) : (data[prevIndex])));

                ++resIndex;
                ++inIndex;
                ++prevIndex;
            }
        }

        /// <summary>
        /// Modifies a row by the Average algorithm.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void CompressAverage(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            long prevIndex = inIndex - inBPR;

            result[resIndex] = 3; // Set the filter type.
            ++resIndex;

            for (int i = 0; i < inBPR; ++i)
            {
                result[resIndex] = (byte)(data[inIndex]
                    - (((i > 0) ? (data[inIndex - 1]) : (m_zero))
                    + ((prevIndex < 0) ? (m_zero) : (data[prevIndex]))) >> 1);

                ++resIndex;
                ++inIndex;
                ++prevIndex;
            }
        }

        /// <summary>
        /// Modifies a row by the Paeth algorithm.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void CompressPaeth(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            long prevIndex = inIndex - inBPR;

            result[resIndex] = 3; // Set the filter type.
            ++resIndex;

            for (int i = 0; i < inBPR; ++i)
            {
                byte a = ((i > 0) ? (data[inIndex - 1]) : (m_zero));
                byte b = ((prevIndex < 0) ? (m_zero) : (data[prevIndex]));
                byte c = ((prevIndex < 1) ? (m_zero) : (data[prevIndex - 1]));

                result[resIndex] = (byte)(data[inIndex]
                    - PaethPredictor(a, b, c));

                ++resIndex;
                ++inIndex;
                ++prevIndex;
            }
        }

        /// <summary>
        /// Retrieves original data for a single row.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void Decompress(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            switch ((Type)data[inIndex])
            {
                case Type.None:
                    DecompressNone(data, inIndex + 1, inBPR, result, resIndex, resBPR);
                    break;

                case Type.Sub:
                    DeompressSub(data, inIndex + 1, inBPR, result, resIndex, resBPR);
                    break;

                case Type.Up:
                    DecompressUp(data, inIndex + 1, inBPR, result, resIndex, resBPR);
                    break;

                case Type.Average:
                    DecompressAverage(data, inIndex + 1, inBPR, result, resIndex, resBPR);
                    break;

                case Type.Paeth:
                    DecompressPaeth(data, inIndex + 1, inBPR, result, resIndex, resBPR);
                    break;

                default:
                    throw new ArgumentException("Unsupported PNG filter: " + data[inIndex].ToString(),
                        "type");
            }
        }

        /// <summary>
        /// Decompresses a row using none filter.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void DecompressNone(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            for (int i = 1; i < inBPR; ++i)
            {
                result[resIndex] = data[inIndex];

                ++resIndex;
                ++inIndex;
            }
        }

        /// <summary>
        /// Decompresses a row using the Sub filter.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void DeompressSub(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            for (int i = 0; i < resBPR; ++i)
            {
                result[resIndex] = (byte)(data[inIndex]
                    + ((i > 0) ? (result[resIndex - 1]) : (m_zero)));

                ++resIndex;
                ++inIndex;
            }
        }

        /// <summary>
        /// Decompresses a row compressed by the Up filter.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static byte[] DecompressUp(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            long prevIndex = resIndex - resBPR;

            for (int i = 0; i < resBPR; ++i)
            {
                result[resIndex] = (byte)(data[inIndex] +
                    ((prevIndex < 0) ? (m_zero) : (result[prevIndex])));

                ++resIndex;
                ++inIndex;
                ++prevIndex;
            }

            return result;
        }

        /// <summary>
        /// Decompress a row compressed by the Average algorithm.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void DecompressAverage(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            long prevIndex = inIndex - inBPR;

            for (int i = 0; i < inBPR; ++i)
            {
                result[resIndex] = (byte)(data[inIndex]
                     + (((i > 0 && (inIndex - 1) < result.Length) ? (result[inIndex - 1]) : (m_zero))
                     + ((prevIndex < 0) ? (m_zero) : ((prevIndex < result.Length) ? result[prevIndex] : (m_zero)))) >> 1);

                ++resIndex;
                ++inIndex;
                ++prevIndex;
            }
        }

        /// <summary>
        /// Unpacks a row packed by the Paeth algorithm.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private static void DecompressPaeth(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR)
        {
            long prevIndex = inIndex - inBPR;

            for (int i = 0; i < inBPR; ++i)
            {
                byte a = ((i > 0) ? (result[inIndex - 1]) : (m_zero));
                byte b = ((prevIndex < 0) ? (m_zero) : (result[prevIndex]));
                byte c = ((prevIndex < 1) ? (m_zero) : (result[prevIndex - 1]));

                result[resIndex] = (byte)(data[inIndex]
                    + PaethPredictor(a, b, c));

                ++resIndex;
                ++inIndex;
                ++prevIndex;
            }
        }

        /// <summary>
        /// Returns the Paeth predictor.
        /// </summary>
        /// <param name="a">Left pixel.</param>
        /// <param name="b">Above pixel.</param>
        /// <param name="c">Upper left pixel.</param>
        /// <returns>The value of the Paeth predictor.</returns>
        private static byte PaethPredictor(byte a, byte b, byte c)
        {
            // a = left, b = above, c = upper left
            int p = a + b - c;    // initial estimate
            int pa = Math.Abs(p - a);  // distances to a, b, c
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);
            // return nearest of a,b,c,
            // breaking ties in order a,b,c.
            if (pa <= pb && pa <= pc)
            {
                return a;
            }
            else if (pb <= pc)
            {
                return b;
            }
            else
            {
                return c;
            }
        }
        #endregion

        #region Class delegates
        /// <summary>
        /// Used to call a row filter.
        /// </summary>
        /// <param name="data">The original data buffer.</param>
        /// <param name="inIndex">The current row index within the original buffer.</param>
        /// <param name="inBPR">The byte-per-row value of the original buffer.</param>
        /// <param name="result">The result data buffer.</param>
        /// <param name="resIndex">The current row index within the result buffer.</param>
        /// <param name="resBPR">The byte-per-row value of the result buffer.</param>
        private delegate void RowFilter(byte[] data, long inIndex, int inBPR,
            byte[] result, long resIndex, int resBPR);
        #endregion
    }
}
