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

#region file using directives

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// The HTML convertor factory class.
  /// </summary>
  public class HtmlConverterFactory
  {
    #region Class members

    /// <summary>
    /// 
    /// </summary>
    [ThreadStatic]
    private static IHtmlConverter s_htmlConverter = null;
    #endregion

    #region Class static methods

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static IHtmlConverter GetInstance()
    {
      if (s_htmlConverter == null)
      {
        throw new HtmlConverterRegisterException();
      }
      return s_htmlConverter;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="converter"></param>
    public static void Register(IHtmlConverter converter)
    {
      if (converter == null)
        throw new ArgumentNullException("convertor");

      s_htmlConverter = converter;
    }

    #endregion
  }

  /// <summary>
  /// 
  /// </summary>
  public interface IHtmlConverter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dlsTextBody"></param>
    /// <param name="html"></param>
    /// <param name="paragraphIndex"></param>
    /// <param name="paragraphItemIndex"></param>
    void AppendToTextBody(ITextBody dlsTextBody, string html, int paragraphIndex, int paragraphItemIndex);
  }
}