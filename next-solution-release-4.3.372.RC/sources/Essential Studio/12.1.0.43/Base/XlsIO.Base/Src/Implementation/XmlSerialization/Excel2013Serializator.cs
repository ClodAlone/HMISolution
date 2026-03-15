#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Xml;



using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;


#if (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
#else

#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
    public class Excel2013Serializator:Excel2010Serializator
    {
        
    #region Constants
    private const string VersionValue = "15.0300";
    #endregion

    /// <summary>
    /// Gets version that is supported by this serializator.
    /// </summary>
    public override ExcelVersion Version
    {
      get
      {
        return ExcelVersion.Excel2013;
      }
    }

    public Excel2013Serializator(WorkbookImpl book) :
      base( book )
    {
    }
    protected override void SerializeAppVersion(XmlWriter writer)
    {
        SerializeElementString(writer, DocProp.AppVersion, VersionValue, null);
    }
    }

}
