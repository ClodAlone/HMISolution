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
using System.ComponentModel;
#endregion

namespace Syncfusion.Shared.Utils.KeyBinding.Implementation
{
  /// <summary>
  /// Summary description for CommandAttribute.
  /// </summary>
  [ AttributeUsage( AttributeTargets.Method, AllowMultiple = false ) ]
  public class CommandAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// Name of the command.
    /// </summary>
    private string m_strName;
    #endregion

    #region Class properties
    /// <summary>
    /// Name of the command.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Hide default constructor from end user.
    /// </summary>
    private CommandAttribute()
      : base()
    {
    }
    /// <summary>
    /// Publish method as Command for Keys binding.
    /// </summary>
    /// <param name="name">Unique name which identify method as command 
    /// for keys binding</param>
    /// <exception cref="ArgumentNullException">name can not be NULL</exception>
    /// <exception cref="ArgumentException">name can not be empty</exception>
    public CommandAttribute( string name )
      : this()
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if( name.Length == 0 )
        throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_14 );

      m_strName = name;
    }
    #endregion
  }
}