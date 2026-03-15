#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;


namespace Syncfusion.XlsIO.Implementation
{
  public class BitConverterGeneral
  {
    //public static unsafe long DoubleToInt64Bits(double value)
    //{
    //  return *(((long*)&value));
    //}
    public static long DoubleToInt64Bits(double value)
    {
#if !SILVERLIGHT && !WINRT
      return BitConverterGeneral.DoubleToInt64Bits( value );
#else
      return BitConverter.ToInt64( BitConverter.GetBytes( value ), 0 );
#endif
    }

    public static double Int64BitsToDouble(long value)
    {
#if !SILVERLIGHT && !WINRT
      return BitConverterGeneral.Int64BitsToDouble( value );
#else
      return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
#endif
    }
  }
}
