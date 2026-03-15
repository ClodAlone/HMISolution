#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.util
{
    internal class ArrayUtil
    {
        public const int MAX_EL_COPYING = 8;
        public const int INIT_EL_COPYING = 4;
        public static void intArraySet(int[] arr, int val)
        {
            int i, len, len2;
            len = arr.Length;
            if (len < MAX_EL_COPYING)
            {
                for (i = len - 1; i >= 0; i--)
                {
                    arr[i] = val;
                }
            }
            else
            {
                len2 = len >> 1;
                for (i = 0; i < INIT_EL_COPYING; i++)
                {
                    arr[i] = val;
                }
                for (; i <= len2; i <<= 1)
                {
                    Array.Copy(arr, 0, arr, i, i);
                }
                if (i < len)
                {
                    Array.Copy(arr, 0, arr, i, len - i);
                }
            }
        }
        public static void byteArraySet(byte[] arr, byte val)
        {
            int i, len, len2;
            len = arr.Length;
            if (len < MAX_EL_COPYING)
            {
                for (i = len - 1; i >= 0; i--)
                {
                    arr[i] = val;
                }
            }
            else
            {
                len2 = len >> 1;
                for (i = 0; i < INIT_EL_COPYING; i++)
                {
                    arr[i] = val;
                }
                for (; i <= len2; i <<= 1)
                {
                    Array.Copy(arr, 0, arr, i, i);
                }
                if (i < len)
                {
                    Array.Copy(arr, 0, arr, i, len - i);
                }
            }
        }
    }
}