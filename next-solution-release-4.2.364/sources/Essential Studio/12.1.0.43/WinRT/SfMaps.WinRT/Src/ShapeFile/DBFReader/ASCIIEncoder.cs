#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Net;
    using System.Windows;    
    using System.Text;


    internal class ASCIIEncoder
    {
        public ASCIIEncoder()
        {
        }
        public static char GetChar(byte byteArray)
        {
            char result = (char)byteArray;
            return result;
        }

        public static string GetString(byte[] byteArray)
        {
            StringBuilder strBild = new StringBuilder();
            for (int i = 0; i <= byteArray.Length - 1; i++)
            {
                if (byteArray[i] != 0)
                {
                    strBild.Append((char)byteArray[i]);
                }
            }
            return strBild.ToString().Trim();
        }
        public static byte[] GetBytes(char[] chars)
        {
            byte[] result = new byte[chars.Length];
            for (int i = 0; i <= chars.Length - 1;i++ )
            {
                result[i]=(byte)chars[i];
            }
            return result;
        }

    }
}
