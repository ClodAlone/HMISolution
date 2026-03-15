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
using System.Diagnostics;

using Microsoft.Vsa;
#endregion

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Describes a reference added to the script engine.
  /// </summary>
  public class CSharpVsaReferenceItem
    : CSharpVsaItem
      , IVsaReferenceItem
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Construct a CSharpVsaReferenceItem given a script engine and item name
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="name"></param>
    public CSharpVsaReferenceItem( CSharpScriptEngine engine, string name )
      : base( engine, name )
    {
    }
    #endregion

    #region IVsaReferenceItem members
    /// <summary>
    /// Gets or sets the name of the referenced assembly.
    /// </summary>
    public string AssemblyName
    {
      get
      {
        if( m_engine.IsClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        return this.Name;
      }
      set
      {
        if( m_engine.IsClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_engine.IsBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( value != m_name )
        {
          if( value == null || value.Trim() == string.Empty )
          {
            throw new VsaException( VsaError.AssemblyNameInvalid );
          }

          this.Name = value;
        }
      }
    }
    #endregion

    #region CSharpVsaItem overrides
    /// <summary>
    /// Gets the specified object's type, as determined by the IVsaItems.CreateItem method.
    /// </summary>
    public override VsaItemType ItemType
    {
      get
      {
        return VsaItemType.Reference;
      }
    }
    #endregion
  }
}