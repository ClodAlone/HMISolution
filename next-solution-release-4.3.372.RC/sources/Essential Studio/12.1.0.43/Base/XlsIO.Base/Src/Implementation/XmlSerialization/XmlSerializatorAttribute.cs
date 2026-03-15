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

using System;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
    ///<exclude/>
	/// <summary>
	/// Class used for serializing Xml Attribute.
	/// </summary>
	[ AttributeUsage( AttributeTargets.Class ) ]
	public sealed class XmlSerializatorAttribute : Attribute
	{
    #region Class members
    /// <summary>
    /// Xml serializator save type.
    /// </summary>
    private ExcelXmlSaveType m_saveType;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Prevents a default instance of the XmlSerializatorAttribute class from being created.	
    /// </summary>
    private XmlSerializatorAttribute()
    {
    }
    /// <summary>
    /// Initializes a new instance of the XmlSerializatorAttribute class.
    /// </summary>
    /// <param name="saveType">Save type.</param>
    public XmlSerializatorAttribute( ExcelXmlSaveType saveType )
    {
      m_saveType = saveType;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets xml serializator save type. Read-only.
    /// </summary>
    public ExcelXmlSaveType SaveType
    {
      get
      {
        return m_saveType;
      }
    }
    #endregion
  }
}
