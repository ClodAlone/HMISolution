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
using System.Collections;
using System.Xml;
using System.Diagnostics;

using Syncfusion.HTMLUI.Base.Utility;
#endregion

namespace Syncfusion.HTMLUI.Base.Parser.HTML
{
	/// <summary>
	/// This class interprets one HTML element. It is used by pre-parser for
	/// converting HTML to XHTML.
	/// </summary>
  internal class HTMLElement
  {
    #region Class members
    /// <summary>
    /// Element's start tag string.
    /// </summary>
    private string  m_strStartTag;
    /// <summary>
    /// Element's end tag string.
    /// </summary>
    private string  m_strEndTag;
    /// <summary>
    /// Element's name.
    /// </summary>
    private string  m_strName;
    /// <summary>
    /// Storage of IsCDATA property.
    /// </summary>
    private bool    m_bIsCData;
    /// <summary>
    /// Children of elements.
    /// </summary>
    private IDictionary m_children;
    /// <summary>
    /// Collection of optional elements.
    /// </summary>
    private IDictionary m_childrenOptionalStart;
    /// <summary>
    /// xPath with special settings.
    /// </summary>
    private string m_expression;
    /// <summary>
    /// Indicates whether whitespaces must be deleted in element.
    /// </summary>
    private bool m_preserveWhitespace;
    #endregion

    #region Class static Properties
    /// <summary>
    /// Returns an empty element which is not valid and cannot be used for any work.
    /// This value can be used as a NULL value replacer.
    /// </summary>
    public static HTMLElement Empty
    {
      get
      {
        return new HTMLElement();
      }
    }
    /// <summary>
    /// Returns xPath with special settings.
    /// </summary>
    public string Expression
    {
      get
      {
        return m_expression;
      }
      set
      {
        if( m_expression != value )
        {
          m_expression = value;
        }
      }
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Gets or sets the tag start string.
    /// </summary>
    public string TagStart
    {
      get
      {
        return m_strStartTag;
      }
      set
      {
        m_strStartTag = value;
      }
    }

    /// <summary>
    /// Gets or sets the tag end string.
    /// </summary>
    public string TagEnd
    {
      get
      {
        return m_strEndTag;
      }
      set
      {
        m_strEndTag = value;
      }
    }

    /// <summary>
    /// Returns the name of the element. Read-only.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
    }
    /// <summary>
    /// Returns the subname of the element name == Name.Substring(5).
    /// </summary>
    public string SubName
    {
      get
      {
        return m_strName.Substring( 5 );
      }
    }
    /// <summary>
    /// Indicates whether element is a CDATA holder.
    /// </summary>
    public bool   IsCDATA
    {
      get
      {
        return m_bIsCData;
      }
      set
      {
        m_bIsCData = value;
      }
    }

    /// <summary>
    /// Collection of child elements. To check if collection contains any
    /// data, use IsChildrenUsed property. It does not create a collection. By default,
    /// element does not contain any collections and only on demand user will create one.
    /// </summary>
    public IDictionary Children
    {
      get
      {
        if( m_children == null )
          m_children = new SortedListEx();

        return m_children;
      }
    }

    /// <summary>
    /// Collection of optional elements. To check if collection contains any
    /// data, use IsOptionalUsed property. It does not create a collection. By default,
    /// element does not contains any collections and only on demand user will create one.
    /// </summary>
    public IDictionary Optional
    {
      get
      {
        if( m_childrenOptionalStart == null )
          m_childrenOptionalStart = new SortedListEx();

        return m_childrenOptionalStart;
      }
    }
    /// <summary>
    /// Indicates whether optional collection is used.
    /// </summary>
    public bool IsOptionalUsed
    {
      get
      {
        return ( m_childrenOptionalStart != null && m_childrenOptionalStart.Count > 0 );
      }
    }
    /// <summary>
    /// Indicates whether child collection contains. True if data is available.
    /// </summary>
    public bool IsChildrenUsed
    {
      get
      {
        return ( m_children != null && m_children.Count > 0 );
      }
    }
    /// <summary>
    /// Indicates whether we have to delete whitespaces in element.
    /// </summary>
    public bool PreserveWhiteSpaces
    {
      get
      {
        return m_preserveWhitespace;
      }
      set
      {
        if( m_preserveWhitespace != value )
        {
          m_preserveWhitespace = value;
        }
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Private constructor to prevent empty class construction.
    /// </summary>
    private HTMLElement()
    {
      m_preserveWhitespace = false;
    }

    /// <summary>
    /// Main constructor used for element initializing.
    /// </summary>
    /// <param name="name">Name of the element.</param>
    public HTMLElement( string name )
      : this()
    {
      m_strName = name;
      m_expression = string.Empty;
    }
    #endregion
  }
}
