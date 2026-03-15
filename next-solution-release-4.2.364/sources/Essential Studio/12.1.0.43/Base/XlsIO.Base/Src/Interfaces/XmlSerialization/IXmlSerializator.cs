#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Xml;

namespace Syncfusion.XlsIO.Interfaces.XmlSerialization
{
  /// <summary>
  /// This interface must be implemented by all xml serializators.
  /// </summary>
  public interface IXmlSerializator
	{
    /// <summary>
    /// Saves workbook into writer.
    /// </summary>
    /// <param name="writer">Writer to save workbook into.</param>
    /// <param name="book">Workbook to save.</param>
    void Serialize( XmlWriter writer, IWorkbook book );
  }
}
