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

using System.Diagnostics;

using Microsoft.Vsa;

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Summary description for CSharpVsaError.
  /// </summary>
  public class CSharpVsaError : IVsaError
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private IVsaItem m_sourceItem;

    /// <summary>
    /// 
    /// </summary>
    private string m_sourceMoniker;

    /// <summary>
    /// 
    /// </summary>
    private int m_startColumn;

    /// <summary>
    /// 
    /// </summary>
    private string m_descr;

    /// <summary>
    /// 
    /// </summary>
    private int m_endColumn;

    /// <summary>
    /// 
    /// </summary>
    private int m_severity;

    /// <summary>
    /// 
    /// </summary>
    private int m_number;

    /// <summary>
    /// 
    /// </summary>
    private int m_line;

    /// <summary>
    /// 
    /// </summary>
    private string m_lineText;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    private CSharpVsaError()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public CSharpVsaError( IVsaItem sourceItem, string sourceMoniker, int startColumn,
                           string descr, int endColumn, int severity, int number, int line, string lineText )
    {
      m_sourceItem = sourceItem;
      m_sourceMoniker = sourceMoniker;
      m_startColumn = startColumn;
      m_descr = descr;
      m_endColumn = endColumn;
      m_severity = severity;
      m_number = number;
      m_line = line;
      m_lineText = lineText;
    }
    #endregion

    #region Class properties
    public IVsaItem SourceItem
    {
      get
      {
        return m_sourceItem;
      }
    }

    public string SourceMoniker
    {
      get
      {
        return m_sourceMoniker;
      }
    }

    public int StartColumn
    {
      get
      {
        return m_startColumn;
      }
    }

    public string Description
    {
      get
      {
        return m_descr;
      }
    }

    public int EndColumn
    {
      get
      {
        return m_endColumn;
      }
    }

    public int Severity
    {
      get
      {
        return m_severity;
      }
    }

    public int Number
    {
      get
      {
        return m_number;
      }
    }

    public int Line
    {
      get
      {
        return m_line;
      }
    }

    public string LineText
    {
      get
      {
        return m_lineText;
      }
    }
    #endregion
  }
}