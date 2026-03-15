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
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Security.Policy;
using System.Text;

using Microsoft.CSharp;
using Microsoft.Vsa;
using System.IO;
#endregion

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Summary description for CSharpScriptEngine.
  /// </summary>
  public class CSharpScriptEngine : IVsaEngine
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_LANGUAGE = "CSharp";
    /// <summary>
    /// Name of the global class for accessing to global objects.
    /// </summary>
    private const string DEF_GLOBAL_NAME = "Global";
    /// <summary>
    /// Whitespace keyword.
    /// </summary>
    private const string DEF_WHITESPACE = " ";
    /// <summary>
    /// Open brace keyword.
    /// </summary>
    private const string DEF_BRACE_OPEN = "{";
    /// <summary>
    /// Close brace keyword.
    /// </summary>
    private const string DEF_BRACE_CLOSE = "}";
    /// <summary>
    /// Source of global class for accerssing global objects.
    /// </summary>
    private const string DEF_GLOBAL_CLASS_SOURCE1 = "namespace {0}";
    /// <summary>
    /// Source of global class for accerssing global objects.
    /// </summary>
    private const string DEF_GLOBAL_CLASS_SOURCE2 = " { using System;\n " +
      "public sealed class " + DEF_GLOBAL_NAME + "{" +
      "\nprivate " + DEF_GLOBAL_NAME + "(){" +
      "throw new NotImplementedException(); }";
    /// <summary>
    /// String for adding global variables.
    /// </summary>
    private const string DEF_GLOBAL_VAR = "\n  public static {0} {1} = null;";
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private CodeDomProvider m_codeProvider;

    /// <summary>
    /// 
    /// </summary>
    private ICodeCompiler m_codeCompiler;

    /// <summary>
    /// 
    /// </summary>
    private bool m_isCompiled;

    /// <summary>
    /// 
    /// </summary>
    private bool m_isRunning;

    /// <summary>
    /// 
    /// </summary>
    private bool m_isClosed;

    /// <summary>
    /// 
    /// </summary>
    private bool m_isInitialized;

    /// <summary>
    /// 
    /// </summary>
    private bool m_isBusy = false;

    /// <summary>
    /// 
    /// </summary>
    private string m_sRootMoniker = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    private string m_sRootNamespace = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    private string m_sName = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    private CSharpVsaItems m_vsaItems;

    /// <summary>
    /// 
    /// </summary>
    private byte[] m_pe = null;

    /// <summary>
    /// 
    /// </summary>
    private byte[] m_pdb = null;

    /// <summary>
    /// 
    /// </summary>
    private CompilerParameters m_compilerParams;

    /// <summary>
    /// 
    /// </summary>
    private Assembly m_compiledAssembly;

    /// <summary>
    /// 
    /// </summary>
    private IVsaSite m_vsaSite = null;

    /// <summary>
    /// 
    /// </summary>
    private Hashtable m_hashOptions = new Hashtable();

    /// <summary>
    /// 
    /// </summary>
    private AppDomain m_appDomain;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    internal bool IsClosed
    {
      get
      {
        return m_isClosed;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    internal bool IsInitialized
    {
      get
      {
        return m_isInitialized;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    internal bool IsBusy
    {
      get
      {
        return m_isBusy;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public CSharpScriptEngine()
    {
      m_codeProvider = new CSharpCodeProvider();
      m_vsaItems = new CSharpVsaItems( this );
      m_compilerParams = new CompilerParameters();

      m_compilerParams.CompilerOptions = "/target:library";
      m_compilerParams.CompilerOptions += " /optimize";
      m_compilerParams.GenerateExecutable = false;
      m_compilerParams.GenerateInMemory = true;
      m_compilerParams.IncludeDebugInformation = false;
    }
    #endregion

    #region IVsaEngine properties
    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    public virtual bool GenerateDebugInfo
    {
      get
      {
        // TODO:  Add CodeDOMEngine.GenerateDebugInfo getter implementation
        return false;
      }
      set
      {
        // TODO:  Add CodeDOMEngine.GenerateDebugInfo setter implementation
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Sets or gets the root namespace used by the script engine
    /// </summary>
    public virtual string RootNamespace
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        return m_sRootNamespace;
      }
      set
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        //if( !m_isInitialized )
        //  throw new VsaException( VsaError.EngineNotInitialized );

        if( !this.IsValidIdentifier( value ) )
        {
          throw new VsaException( VsaError.RootNamespaceInvalid );
        }

        m_sRootNamespace = value;
      }
    }

    /// <summary>
    /// Gets a Boolean value that signifies whether a script engine 
    /// has successfully compiled the current source state
    /// </summary>
    public virtual bool IsCompiled
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( !m_isInitialized )
        {
          throw new VsaException( VsaError.EngineNotInitialized );
        }

        return m_isCompiled;
      }
    }

    /// <summary>
    /// Sets or gets a script engine's root moniker.
    /// </summary>
    public virtual string RootMoniker
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        return m_sRootMoniker;
      }
      set
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( m_sRootMoniker != string.Empty )
        {
          throw new VsaException( VsaError.RootMonikerAlreadySet );
        }

        // TODO: impl. RootMonikerInvalid, RootMonikerInUse, 
        // RootMonikerProtocolInvalid - exceptions !!!

        m_sRootMoniker = value;

        // test !
        m_isInitialized = true;
      }
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    public virtual int LCID
    {
      get
      {
        // TODO:  Add CodeDOMEngine.LCID getter implementation
        throw new NotImplementedException();
      }
      set
      {
        // TODO:  Add CodeDOMEngine.LCID setter implementation
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets a compiled assembly
    /// </summary>
    public virtual Assembly Assembly
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( !m_isRunning )
        {
          throw new VsaException( VsaError.EngineNotRunning );
        }

        return m_compiledAssembly;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual IVsaItems Items
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( !m_isInitialized )
        {
          throw new VsaException( VsaError.EngineNotInitialized );
        }

        return m_vsaItems;
      }
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    public virtual Evidence Evidence
    {
      get
      {
        // TODO:  Add CodeDOMEngine.Evidence getter implementation
        throw new NotImplementedException();
      }
      set
      {
        // TODO:  Add CodeDOMEngine.Evidence setter implementation
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Sets or gets the display name of the script engine that 
    /// is used primarily for identifying individual script engines 
    /// to users in a hosted environment
    /// </summary>
    public virtual string Name
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( !m_isInitialized )
        {
          throw new VsaException( VsaError.EngineNotInitialized );
        }

        return m_sName;
      }
      set
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( m_isRunning )
        {
          throw new VsaException( VsaError.EngineRunning );
        }

        if( !m_isInitialized )
        {
          throw new VsaException( VsaError.EngineNotInitialized );
        }

        m_sName = value;
      }
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    public virtual string Version
    {
      get
      {
        // TODO:  Add CodeDOMEngine.Version getter implementation
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets a Boolean value that reports whether 
    /// the script engine is currently in run mode
    /// </summary>
    public virtual bool IsRunning
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( !m_isInitialized )
        {
          throw new VsaException( VsaError.EngineNotInitialized );
        }

        return m_isRunning;
      }
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    public virtual bool IsDirty
    {
      get
      {
        // TODO:  Add CodeDOMEngine.IsDirty getter implementation
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets the nonlocalized name of the 
    /// programming language supported by the script engine
    /// </summary>
    public virtual string Language
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( !m_isInitialized )
        {
          throw new VsaException( VsaError.EngineNotInitialized );
        }

        return DEF_LANGUAGE;
      }
    }

    /// <summary>
    /// Sets or gets the host-implemented IVsaSite object 
    /// that is used by the script engine to communicate with the host
    /// </summary>
    public virtual IVsaSite Site
    {
      get
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        return m_vsaSite;
      }
      set
      {
        if( m_isClosed )
        {
          throw new VsaException( VsaError.EngineClosed );
        }

        if( m_isBusy )
        {
          throw new VsaException( VsaError.EngineBusy );
        }

        if( m_sRootMoniker == string.Empty )
        {
          throw new VsaException( VsaError.RootMonikerNotSet );
        }

        if( m_vsaSite != null )
        {
          throw new VsaException( VsaError.SiteAlreadySet );
        }

        // TODO: impl. SiteInvalid exception !!!

        m_vsaSite = value;
        m_isInitialized = true;
      }
    }
    #endregion

    #region IVsaEngine methods
    /// <summary>
    /// 
    /// </summary>
    public virtual void Reset()
    {
      if( m_isClosed )
      {
        throw new VsaException( VsaError.EngineClosed );
      }

      if( m_isBusy )
      {
        throw new VsaException( VsaError.EngineBusy );
      }

      if( !m_isRunning )
      {
        throw new VsaException( VsaError.EngineNotRunning );
      }

      // TODO: impl. EngineCannotReset exception

      m_compiledAssembly = null;
      m_isRunning = false;
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    /// <param name="site"></param>
    public virtual void SaveSourceState( IVsaPersistSite site )
    {
      // TODO:  Add CodeDOMEngine.SaveSourceState implementation
      //m_persistSite = site;
      throw new NotImplementedException();
    }

    /// <summary>
    /// Saves the compiled state of the script engine; 
    /// optionally, it also saves debugging information
    /// </summary>
    /// <param name="pe"></param>
    /// <param name="pdb"></param>
    public virtual void SaveCompiledState( out byte[] pe, out byte[] pdb )
    {
      if( m_isClosed )
      {
        throw new VsaException( VsaError.EngineClosed );
      }

      if( m_isBusy )
      {
        throw new VsaException( VsaError.EngineBusy );
      }

      if( m_isRunning )
      {
        throw new VsaException( VsaError.EngineRunning );
      }

      if( !m_isCompiled )
      {
        throw new VsaException( VsaError.EngineNotCompiled );
      }

      // TODO:  impl. SaveCompiledStateFailed exception

      pe = m_pe;
      pdb = m_pdb;
    }

    /// <summary>
    /// Gets implementation-specific options for a script engine.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual object GetOption( string name )
    {
      return m_hashOptions[ name ];
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    public virtual void InitNew()
    {
      // TODO:  Add CodeDOMEngine.InitNew implementation
      //throw new NotImplementedException();
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    public virtual void RevokeCache()
    {
      // TODO:  Add CodeDOMEngine.RevokeCache implementation
      throw new NotImplementedException();
    }

    /// <summary>
    /// Closes the script engine and releases all resources. 
    /// If the script engine is currently running, 
    /// the IVsaEngine.Reset method is called first.
    /// </summary>
    public virtual void Close()
    {
      if( m_isClosed )
      {
        throw new VsaException( VsaError.EngineClosed );
      }

      if( m_isBusy )
      {
        throw new VsaException( VsaError.EngineBusy );
      }

      if( m_isRunning )
      {
        this.Reset();
      }

      m_isClosed = true;

      // TODO: impl. EngineCannotClose exception
    }

    /// <summary>
    /// Causes the script engine to compile the existing source state
    /// </summary>
    /// <returns></returns>
    public virtual bool Compile()
    {
      if( m_isClosed )
      {
        throw new VsaException( VsaError.EngineClosed );
      }

      if( m_isBusy )
      {
        throw new VsaException( VsaError.EngineBusy );
      }

      if( !m_isInitialized )
      {
        throw new VsaException( VsaError.EngineNotInitialized );
      }

      if( m_isRunning )
      {
        throw new VsaException( VsaError.EngineRunning );
      }

      if( m_sRootNamespace == string.Empty )
      {
        throw new VsaException( VsaError.RootNamespaceNotSet );
      }

      // TODO: impl. AssemblyExpected exception
      // Descr: "One of the IVsaReferenceItem objects is not a valid assembly"
      // Also impl. GlobalInstanceTypeInvalid, EventSourceTypeInvalid exceptions

      m_isCompiled = false;

      // Create code compiler
      m_codeCompiler = m_codeProvider.CreateCompiler();

      // Load references
      LoadReferences();

      // Compile assembly 
      CompilerResults results = GetCompilerResults();

      // Do we have any compiler errors
      if( results.Errors.Count > 0 )
      {
        foreach( CompilerError error in results.Errors )
        {
          // TO DO: impl
          IVsaError vsaError = new CSharpVsaError(
            null, "sourceMoniker", error.Column, error.ErrorText, error.Column, 0, 0,
            error.Line, string.Empty );

          m_vsaSite.OnCompilerError( vsaError );
        }
      }
      else
      {
        m_compiledAssembly = results.CompiledAssembly;
        m_isCompiled = true;
      }

      return m_isCompiled;
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    /// <param name="site"></param>
    public virtual void LoadSourceState( IVsaPersistSite site )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Initiates execution of compiled code in the script engine 
    /// and binds all event handlers.
    /// </summary>
    public virtual void Run()
    {
      if( m_isClosed )
      {
        throw new VsaException( VsaError.EngineClosed );
      }

      if( m_isBusy )
      {
        throw new VsaException( VsaError.EngineBusy );
      }

      if( m_isRunning )
      {
        throw new VsaException( VsaError.EngineRunning );
      }

      if( m_sRootMoniker == string.Empty )
      {
        throw new VsaException( VsaError.RootMonikerNotSet );
      }

      if( m_vsaSite == null )
      {
        throw new VsaException( VsaError.SiteNotSet );
      }

      // TODO: impl. AppDomainInvalid, GetCompiledStateFailed, CachedAssemblyInvalid exceptions

      m_isRunning = false;

      if( m_appDomain != null )
      {
        AssemblyName assmName = m_compiledAssembly.GetName();

        try
        {
          m_appDomain.Load( assmName );
        }
        catch( Exception ex )
        {
#if false
          MessageBox.Show( ex.Message );
#else
          throw ex;
#endif
        }

        for( int i = 0, end = m_vsaItems.Count ; i < end ; i++ )
        {
          IVsaReferenceItem refItem = m_vsaItems[ i ] as IVsaReferenceItem;
          if( refItem != null )
          {
            m_appDomain.Load( refItem.AssemblyName );
          }
        }
        Assembly[] appAssemblies = m_appDomain.GetAssemblies();

        for( int i = 0, end = appAssemblies.Length ; i < end ; i++ )
        {
          AssemblyName appAssemblyName = appAssemblies[ i ].GetName();
          AssemblyName compAssemblyName = m_compiledAssembly.GetName();
          if( appAssemblyName == compAssemblyName )
          {
            m_compiledAssembly = appAssemblies[ i ];
            break;
          }
        }
      }

      if( m_compiledAssembly != null )
      {
        Type globalType = m_compiledAssembly.GetType( RootNamespace + "." + DEF_GLOBAL_NAME );

        if( globalType != null )
        {
          FieldInfo[] fileds = globalType.GetFields( BindingFlags.Static );
          fileds = globalType.GetFields();

          for( int i = 0, end = fileds.Length ; i < end ; i++ )
          {
            FieldInfo field = fileds[ i ];
            object value = m_vsaSite.GetGlobalInstance( field.Name );
            field.SetValue( null, value );
          }
        }
      }

      m_isRunning = true;
    }

    /// <summary>
    /// Checks whether the supplied identifier is valid for the script engine.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public virtual bool IsValidIdentifier( string identifier )
    {
      if( m_isClosed )
      {
        throw new VsaException( VsaError.EngineClosed );
      }

      if( m_isBusy )
      {
        throw new VsaException( VsaError.EngineBusy );
      }

      //if( !m_isInitialized )
      //  throw new VsaException( VsaError.EngineNotInitialized );

      // TODO: impl. checks code !!!

      return true;
    }

    /// <summary>
    /// Checks whether the supplied identifier is valid for the script engine
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public virtual void SetOption( string name, object value )
    {
      if( value == null )
      {
        throw new ArgumentNullException( "value" );
      }

      if( name == null )
      {
        throw new ArgumentNullException( "name" );
      }

      if( name.Length == 0 )
      {
        throw new ArgumentException( "name - string can not be empty" );
      }

      m_hashOptions[ name ] = value;

      if( name == "AppDomain" &&
        ( m_appDomain == null || m_appDomain.FriendlyName != ( string )value )
        )
      {
        m_appDomain = AppDomain.CreateDomain( ( string )value );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    private void LoadReferences()
    {
      m_compilerParams.ReferencedAssemblies.Clear();

      foreach( IVsaItem item in m_vsaItems )
      {
        if( item.ItemType == VsaItemType.Reference )
        {
          m_compilerParams.ReferencedAssemblies.Add( item.Name );
        }
      }

      m_compilerParams.ReferencedAssemblies.Add( Assembly.GetExecutingAssembly().Location );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private string GenerateAdditionalCode()
    {
      StringBuilder builder = new StringBuilder();
      builder.AppendFormat( DEF_GLOBAL_CLASS_SOURCE1, this.RootNamespace );
      builder.Append( DEF_GLOBAL_CLASS_SOURCE2 );

      foreach( IVsaItem item in m_vsaItems )
      {
        if( item.ItemType == VsaItemType.AppGlobal )
        {
          CSharpVsaGlobalItem globItem = item as CSharpVsaGlobalItem;

          if( globItem != null )
          {
            builder.AppendFormat( DEF_GLOBAL_VAR,
                                  globItem.GetTypeString(), globItem.Name );
          }
        }
      }

      builder.Append( Environment.NewLine );
      builder.Append( DEF_BRACE_CLOSE );
      builder.Append( DEF_BRACE_CLOSE );

      return builder.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private CompilerResults GetCompilerResults()
    {
      string source = string.Empty;

      foreach( IVsaItem item in m_vsaItems )
      {
        if( item.ItemType == VsaItemType.Code )
        {
          IVsaCodeItem codeItem = item as IVsaCodeItem;
          source = codeItem.SourceText;
          break;
        }
      }

      if( source == string.Empty )
      {
        throw new VsaException( VsaError.EngineEmpty );
      }

      source += GenerateAdditionalCode();
      //m_codeCompiler.
      return m_codeCompiler.CompileAssemblyFromSource( m_compilerParams, source );
    }
    #endregion
  }
}