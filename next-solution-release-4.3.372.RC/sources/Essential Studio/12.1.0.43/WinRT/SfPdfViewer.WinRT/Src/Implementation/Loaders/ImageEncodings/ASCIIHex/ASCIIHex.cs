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
using Syncfusion.Pdf.Primitives;
using System.Collections;
using System.IO;

namespace Syncfusion.Pdf
{
    internal class ASCIIHex
    {
        public ASCIIHex()
        {
        }

        /// <summary>
        /// asciihexdecode using our own implementation
        /// </summary>
        public byte[] Decode(byte[] data)
        {
            string line;
            StringBuilder value = new StringBuilder();
            StringBuilder valuesRead = new StringBuilder();
            System.IO.StreamReader mappingStream = null;
            MemoryStream bis = null;

            // read in ASCII mode to handle line returns
            bis = new MemoryStream(data);
            mappingStream = new System.IO.StreamReader(bis);

            // read values into lookup table
            if (mappingStream != null)
            {
                while (true)
                {
                    line = mappingStream.ReadLine();

                    if (line == null)
                    {
                        break;
                    }
                    // append to data
                    valuesRead.Append(line);
                }
            }

            if (mappingStream != null)
            {
                //mappingStream.Close();
                //bis.Close();
            }

            int data_size = valuesRead.Length;
            int i = 0, count = 0;
            char current;

            MemoryStream bos = new MemoryStream();

            /// <summary>
            /// loop to read and process </summary>
            while (true)
            {
                current = valuesRead[i];
                if ((current >= '0' && current <= '9') || (current >= 'a' && current <= 'f') || (current >= 'A' && current <= 'F'))
                {
                    value.Append(current);
                    if (count == 1)
                    {
                        int intValue = Convert.ToInt32(value.ToString(), 16);
                        byte[] buffer = new byte[4];
                        buffer[0] = (byte)intValue;
                        buffer[1] = (byte)(intValue >> 8);
                        buffer[2] = (byte)(intValue >> 16);
                        buffer[3] = (byte)(intValue >> 24);

                        bos.Write(buffer, 0, 4);
                        count = 0;
                        value = new StringBuilder();
                    }
                    else
                    {
                        count++;
                    }
                }
                if (current == '>')
                {
                    break;
                }
                i++;
                if (i == data_size)
                {
                    break;
                }
            }
            // write any last char
            if (count == 1)
            {
                value.Append('0');
                int intValue = (int)Convert.ToInt32(value.ToString(), 16);
                byte[] buffer = new byte[4];
                buffer[0] = (byte)intValue;
                buffer[1] = (byte)(intValue >> 8);
                buffer[2] = (byte)(intValue >> 16);
                buffer[3] = (byte)(intValue >> 24);

                bos.Write(buffer, 0, 4);
            }
            //bos.Close();
            return bos.ToArray();
        }
    }
}
