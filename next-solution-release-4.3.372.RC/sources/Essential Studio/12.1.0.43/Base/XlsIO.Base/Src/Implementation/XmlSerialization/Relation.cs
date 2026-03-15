#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
	/// <summary>
	/// This class stores relation data (except relation id).
	/// </summary>
	public class Relation : ICloneable
	{
    #region Members
    /// <summary>
    /// Represents Target.
    /// </summary>
    private string m_strTarget;
    /// <summary>
    /// Target type.
    /// </summary>
    private string m_strType;
    /// <summary>
    /// Defines whether target is external or not.
    /// </summary>
    private bool m_bIsExternal;
    #endregion

    #region Constructors
    /// <summary>
    /// Prevents a default instance of the Relation class from being created.
    /// </summary>
    private Relation()
    {
    }
    /// <summary>
    /// Initializes a new instance of the Relation class.
    /// </summary>
    /// <param name="target">Represents target.</param>
    /// <param name="type">Represents destination type.</param>
    public Relation( string target, string type )
    {
      if( target == null )
        throw new ArgumentNullException( "target" );

      if( type == null )
        throw new ArgumentNullException( "type" );

      m_strTarget = target;
      m_strType = type;
		}
    /// <summary>
    /// Initializes a new instance of the Relation class.
    /// </summary>
    /// <param name="target">Represents target.</param>
    /// <param name="type">Represents destination type.</param>
    /// <param name="isExternal">Value indicating whether target is external.</param>
    public Relation( string target, string type, bool isExternal )
      :this( target, type )
    {
      m_bIsExternal = isExternal;
    }

    #endregion

    #region Properties
    /// <summary>
    /// Gets target. Read-only.
    /// </summary>
    public string Target
    {
      get
      {
        return m_strTarget;
      }
    }
    /// <summary>
    /// Gets destination type. Read-only.
    /// </summary>
    public string Type
    {
      get
      {
        return m_strType;
      }
    }
    /// <summary>
    /// Gets a value indicating whether target is external.
    /// </summary>
    public bool IsExternal
    {
      get
      {
        return m_bIsExternal;
      }
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Create copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public object Clone()
    {
      return MemberwiseClone();
    }

    #endregion
  }
}
