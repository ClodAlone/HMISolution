#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Summary description for ImageStyle.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class ImageStyle
    : Style,
      IImageStyle
  {
    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ImageStyle( IDocument doc )
      : base( doc )
    {
    }
    internal ImageStyle( ImageStyle style, IDocument doc )
      : base( style, doc )
    {}
    #endregion
    
    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public override IStyle Clone( IDocument document )
    {
      return new ImageStyle( this, document );
    }
    #endregion
  }
}