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
using System.Drawing;

namespace Syncfusion.XlsIO
{
  public interface IOleObjects : IList<IOleObject>
  {
    /// <summary>
    /// Adds new ole object to the collection.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="image">File image.</param>
    /// <param name="linkType">Link type.</param>
    IOleObject Add( string fileName, Image image, OleLinkType linkType );
  }
}
