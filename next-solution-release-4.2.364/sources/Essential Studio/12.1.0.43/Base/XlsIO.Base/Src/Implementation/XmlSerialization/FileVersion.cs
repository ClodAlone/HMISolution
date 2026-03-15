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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  public class FileVersion
  {
    public string ApplicationName = Excel2007Serializator.ApplicationNameValue;
    public string BuildVersion = "4506";//"4596";
    public string LastEdited = "4";
    public string LowestEdited = "4";
    public string CodeName;
  }
}
