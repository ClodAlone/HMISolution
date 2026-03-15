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
using System.IO;
using System.Collections;

namespace Syncfusion.Pdf
{
    internal class ASCII85
    {
        private long[] hex_indices = { 256 * 256 * 256, 256 * 256, 256, 1 };
        private long[] base_85_indices = { 85 * 85 * 85 * 85, 85 * 85 * 85, 85 * 85, 85, 1 };
        private int m_specialCases = 0;
        private int m_returns = 0;
        private int m_dataSize = 0;
        private int m_outputPointer = 0;

        public ASCII85()
        {
        }

        /// <summary>
        /// Decodes the ASCII85 encoded byte[]
        /// </summary>
        /// <param name="encodedData">encoded byte[]</param>
        /// <returns>decoded byte[]</returns>
        public byte[] decode(byte[] encodedData)
        {
            m_dataSize = encodedData.Length;

            for (int i = 0; i < m_dataSize; i++)
            {
                if (encodedData[i] == 122)
                {
                    m_specialCases++;
                }
                else if (encodedData[i] == 10)
                {
                    m_returns++;
                }
            }

            
            long value;

            byte[] data = new byte[m_dataSize - m_returns + 1 + (m_specialCases * 3)];
            int j, next;

            for (int i = 0; i < m_dataSize; i++)
            {
                value = 0;
                next = encodedData[i];
                while ((next == 10) || (next == 13))
                {
                    i++;
                    if (i == m_dataSize)
                    {
                        next = 0;
                    }
                    else
                    {
                        next = encodedData[i];
                    }
                }

                if (next == 122)
                {
                    for (int i3 = 0; i3 < 4; i3++)
                    {
                        data[m_outputPointer] = 0;
                        m_outputPointer++;
                    }
                }
                else if ((m_dataSize - i > 4) && (next > 32) && (next < 118))
                {
                    for (j = 0; j < 5; j++)
                    {
                        if (i < encodedData.Length)
                        {
                            next = encodedData[i];
                        }

                        while ((next == 10) || (next == 13))
                        {
                            i++;
                            if (i == m_dataSize)
                            {
                                next = 0;
                            }
                            else
                            {
                                next = encodedData[i];
                            }
                        }
                        i++;
                        if (((next > 32) && (next < 118)) || (next == 126))
                        {
                            value = value + ((next - 33) * base_85_indices[j]);
                        }
                    }

                    for (int k = 0; k < 4; k++)
                    {
                        data[m_outputPointer] = (byte)((value / hex_indices[k]) & 255);
                        m_outputPointer++;
                    }
                    i--;
                }
            }

            byte[] decodedBytes = new byte[m_outputPointer];
            Array.Copy(data, 0, decodedBytes, 0, m_outputPointer);

            return decodedBytes;
        }
    }
}
