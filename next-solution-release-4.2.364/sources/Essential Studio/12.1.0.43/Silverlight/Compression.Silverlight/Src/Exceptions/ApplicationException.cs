#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WITHOUTOWNTYPES
using System;

namespace System
{
  public class ApplicationException : Exception
  {
    public ApplicationException() :
      base()
    {
    }

    public ApplicationException(string message) :
      base(message)
    {
    }

    public ApplicationException( string message, Exception innerException ):
      base( message, innerException )
    {
    }
  }
}
#endif