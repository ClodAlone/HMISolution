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
using System.Reflection;
#endregion

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Describe the property type for showing in ScriptObjectBrowser
  /// </summary>
  public enum PropertyType
  {
    /// <summary>
    /// The common type property
    /// </summary>
    None,
    /// <summary>
    /// Property is a collection
    /// </summary>
    Collection,
    /// <summary>
    /// Property is a item of collection
    /// </summary>
    Item
  }

  /// <summary>
  /// Attribute alow to specify which element of class accessable for Script
  /// engine edit control can be show in model tree.
  /// </summary>
  public class ScriptBrowsableAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// storage of IsVisible property
    /// </summary>
    private bool m_visible;

    /// <summary>
    /// storage of property property
    /// </summary>
    private PropertyType m_propType = PropertyType.None;

    /// <summary>
    /// storage of name property
    /// </summary>
    private string m_itemName = string.Empty;

    /// <summary>
    /// storage of type property
    /// </summary>
    private Type m_itemType;
    #endregion

    #region Class properties
    /// <summary>
    /// Indicated whether class property is visible
    /// </summary>
    public bool IsVisible
    {
      get
      {
        return m_visible;
      }
    }

    /// <summary>
    /// Indicated way for show property.
    /// Readonly
    /// </summary>
    public PropertyType PropertyType
    {
      get
      {
        return m_propType;
      }
    }

    /// <summary>
    /// Gets the name of property that using for showing name of Item.
    /// Readonly
    /// </summary>
    public string Name
    {
      get
      {
        if( m_propType == PropertyType.Collection && m_itemName.Length == 0 )
        {
          ScriptBrowsableAttribute attr = GetFromMemberInfo( m_itemType );

          if( attr != null )
          {
            m_itemName = attr.Name;
          }
        }

        return m_itemName;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Disabel default constructor for end-user
    /// </summary>
    private ScriptBrowsableAttribute()
    {
    }

    /// <summary>
    /// Main constructor for our own use
    /// </summary>
    /// <param name="visible">True - element of class is visible to ScriptEditControl, otherwise False</param>
    public ScriptBrowsableAttribute( bool visible )
    {
      m_visible = visible;
    }

    /// <summary>
    /// Using this constructor for mark "Item" class
    /// </summary>
    /// <param name="property">Type property - must be equal PropertyType.Item</param>
    /// <param name="name">Name of property that using for showing name of Item.</param>
    public ScriptBrowsableAttribute( PropertyType property, string name )
    {
      if( property != PropertyType.Item )
      {
        throw new ArgumentException( "property must equal the PropertyType.Item" );
      }

      m_visible = true;
      m_propType = PropertyType.Item;
      m_itemName = name;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="property"></param>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public ScriptBrowsableAttribute( PropertyType property, Type type, string name )
    {
      if( property != PropertyType.Item )
      {
        throw new ArgumentException( "property must equal the PropertyType.Item" );
      }

      if( type == null )
      {
        throw new ArgumentNullException( "type" );
      }

      m_visible = true;
      m_propType = PropertyType.Item;
      m_itemName = name;
      m_itemType = type;
    }

    /// <summary>
    /// Using this constructor for mark "Colleciton" property
    /// </summary>
    public ScriptBrowsableAttribute( PropertyType property, Type type )
    {
      if( property != PropertyType.Collection )
      {
        throw new ArgumentException( "property must equal the PropertyType.Collection" );
      }

      if( type == null )
      {
        throw new ArgumentNullException( "type" );
      }

      m_visible = true;
      m_propType = PropertyType.Collection;
      m_itemType = type;
    }
    #endregion

    #region Class Public Methods
    public PropertyInfo GetItemNameProperty()
    {
      //if( m_propType != PropertyType.Collection )
      //{
      //  return null;
      //}

      return m_itemType.GetProperty( this.Name );
    }
    #endregion

    #region Class utility methods
    public static ScriptBrowsableAttribute GetFromMemberInfo( MemberInfo info )
    {
      object[] attrs = info.GetCustomAttributes( typeof( ScriptBrowsableAttribute ), true );

      if( attrs == null || attrs.Length == 0 )
      {
        return null;
      }

      return ( ScriptBrowsableAttribute )( attrs[ 0 ] );
    }
    #endregion
  }
}