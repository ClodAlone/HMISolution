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
using System.Collections;
using System.Diagnostics;

using Microsoft.JScript.Vsa;
using Microsoft.Vsa;

namespace Syncfusion.Scripting
{
  /// <summary>
  /// 
  /// </summary>
  public class ScriptEngineFactory
  {
    private static Hashtable scriptEngineTypes = new Hashtable();

    public static ICollection SupportedLanguages
    {
      get
      {
        return scriptEngineTypes.Keys;
      }
    }

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Static Constructor
    /// </summary>
    static ScriptEngineFactory()
    {
      ScriptEngineFactory.RegisterScriptEngine( ScriptLanguages.JScript, typeof( VsaEngine ) );
      ScriptEngineFactory.RegisterScriptEngine( ScriptLanguages.VisualBasic, typeof( Microsoft.VisualBasic.Vsa.VsaEngine ) );
      ScriptEngineFactory.RegisterScriptEngine( ScriptLanguages.CSharp, typeof( CSharpScriptEngine ) );
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="scriptEngineType"></param>
    public static void RegisterScriptEngine( ScriptLanguages lang, Type scriptEngineType )
    {
      if( ScriptEngineFactory.scriptEngineTypes.Contains( lang ) )
      {
        ScriptEngineFactory.scriptEngineTypes[ lang ] = scriptEngineType;
      }
      else
      {
        ScriptEngineFactory.scriptEngineTypes.Add( lang, scriptEngineType );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lang"></param>
    /// <returns></returns>
    public static IVsaEngine CreateScriptEngine( ScriptLanguages lang )
    {
      IVsaEngine scriptEngine = null;

      if( ScriptEngineFactory.scriptEngineTypes.Contains( lang ) )
      {
        Type scriptEngineType = ( Type )ScriptEngineFactory.scriptEngineTypes[ lang ];
        scriptEngine = Activator.CreateInstance( scriptEngineType ) as IVsaEngine;
      }

      return scriptEngine;
    }
  }
}