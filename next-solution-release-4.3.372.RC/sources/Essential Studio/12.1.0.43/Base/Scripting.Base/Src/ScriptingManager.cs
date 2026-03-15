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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Reflection;

using Microsoft.Vsa;

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Encapsulates a script, script site, and script engine as a single component.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class provides a convenient wrapper around a script, script site, and
  /// script engine. It can be added to a form using the toolbox and configured
  /// using the property editor.
  /// </para>
  /// <para>
  /// The ScriptingManager creates the <see cref="Syncfusion.Scripting.ScriptSite"/>
  /// and <see cref="Syncfusion.Scripting.Script"/> objects by calling the virtual
  /// <see cref="Syncfusion.Scripting.ScriptingManager.CreateScriptSite"/> and
  /// <see cref="Syncfusion.Scripting.ScriptingManager.CreateScript"/> methods,
  /// respectively. These methods can be overriden in derived ScriptingManager
  /// classes in order to create custom Script and ScriptSite objects. After
  /// creating the ScriptSite and Script, the ScriptingManager attaches these
  /// objects to the <see cref="Syncfusion.Scripting.ScriptingManager.ScriptSite"/>
  /// and <see cref="Syncfusion.Scripting.ScriptingManager.Script"/> properties,
  /// respectively. Another way to attach a custom ScriptSite or Script to the
  /// ScriptingManager is to pass them to one of the overloaded constructors.
  /// </para>
  /// <para>
  /// The <see cref="Syncfusion.Scripting.ScriptingManager.ScriptEngine"/> is
  /// created by the <see cref="Syncfusion.Scripting.ScriptEngineFactory"/>
  /// based on the value of the
  /// <see cref="Syncfusion.Scripting.ScriptingManager.ScriptLanguage"/>
  /// property.
  /// </para>
  /// <para>
  /// The <see cref="Syncfusion.Scripting.ScriptingManager.LoadScript"/>
  /// method is used to load the script source code. Once the script is
  /// loaded, the
  /// </para>
  /// <para>
  /// <see cref="Syncfusion.Scripting.ScriptingManager.CompileScript"/>
  /// method can be called to compile the script.
  /// </para>
  /// <para>
  /// <see cref="Syncfusion.Scripting.ScriptingManager.CompileScript"/>
  /// method can be called to compile the script.
  /// </para>
  /// </remarks>
  [
  ToolboxItem(false),
  DesignTimeVisible(false)
  ]
  public class ScriptingManager : Component
  {
    #region Constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    public ScriptingManager()
    {
      this.Init();
    }

    /// <summary>
    /// Construct a ScriptingManager given a script site.
    /// </summary>
    /// <param name="scriptSite">Script site to attach</param>
    public ScriptingManager( ScriptSite scriptSite )
    {
      this.scriptSite = scriptSite;
      this.script = script;
      this.Init();
    }

    /// <summary>
    /// Construct a ScriptingManager given a script.
    /// </summary>
    /// <param name="script">Script to attach</param>
    public ScriptingManager( Script script )
    {
      this.script = script;
      this.Init();
    }

    /// <summary>
    /// Construct a ScriptingManager given a script site and a script.
    /// </summary>
    /// <param name="scriptSite">Script site to attach</param>
    /// <param name="script">Script to attach</param>
    public ScriptingManager( ScriptSite scriptSite, Script script )
    {
      this.scriptSite = scriptSite;
      this.script = script;
      this.Init();
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// The ScriptEngine compiles and executes the script.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each language uses a different ScriptEngine. The
    /// <see cref="Syncfusion.Scripting.ScriptEngineFactory"/> creates the
    /// appropriate ScriptEngine for a given language. New or alternate
    /// ScriptEngines can be registered with the
    /// <see cref="Syncfusion.Scripting.ScriptEngineFactory"/>.
    /// </para>
    /// </remarks>
    [
      Browsable( false ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
      ]
    public IVsaEngine ScriptEngine
    {
      get
      {
        return this.scriptEngine;
      }
    }

    /// <summary>
    /// Script to load into the.
    /// <see cref="Syncfusion.Scripting.ScriptingManager.ScriptEngine"/>.
    /// </summary>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
        Editor( "Syncfusion.Scripting.Design.ScriptUITypeEditor, " +
          Library.DesignerVersion, typeof( UITypeEditor ) )
      ]
    public Script Script
    {
      get
      {
        return this.script;
      }
      set
      {
        if( this.script != value )
        {
          if( this.script != null )
          {
            this.script.Changed -= new ScriptEventHandler( this.script_Changed );
          }
          this.script = value;
          if( this.script != null )
          {
            this.script.Changed += new ScriptEventHandler( this.script_Changed );
          }
        }
      }
    }

    /// <summary>
    /// The ScriptSite provides for host-implemented callbacks, describes interaction between
    /// the engine and the host's object model, and allows the reporting of compilation errors.
    /// </summary>
    [
      Browsable( false ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
      ]
    public ScriptSite ScriptSite
    {
      get
      {
        return this.scriptSite;
      }
    }

    /// <summary>
    /// Indicates if the script has been loaded and compiled in the script engine.
    /// </summary>
    [
      Browsable( false ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
      ]
    public bool IsScriptCompiled
    {
      get
      {
        bool value = false;
        if( this.scriptEngine != null )
        {
          value = this.scriptEngine.IsCompiled;
        }
        return value;
      }
    }

    /// <summary>
    /// Indicates if the script engine is currently running the script.
    /// </summary>
    [
      Browsable( false ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
      ]
    public bool IsScriptRunning
    {
      get
      {
        bool value = false;
        if( this.scriptEngine != null )
        {
          value = this.scriptEngine.IsRunning;
        }
        return value;
      }
    }

    /// <summary>
    /// Indicates if the script needs to be compiled.
    /// </summary>
    [
      Browsable( false ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
      ]
    public bool NeedToCompile
    {
      get
      {
        return ( this.scriptDirty || !this.IsScriptCompiled );
      }
    }
		
	  private string applicationBase = string.Empty;
	 
	  /// <summary>
	  /// Sets the root folder where non-system assemblies are located.
	  /// </summary>
	  [
	  Browsable( true ),
	  DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
	  ]
	  public string ApplicationBase
	  {
		  get
		  {
			  return this.applicationBase;
		  }
		  set
		  {
			this.applicationBase = value;
		  }
	  }

    #endregion

    #region Public Methods
    /// <summary>
    /// Loads the contents of the specified file into the script.
    /// </summary>
    /// <param name="fileName">Name of file containing the script source</param>
    public void LoadScript( string fileName )
    {
      if( this.script == null )
      {
        throw new InvalidOperationException();
      }

      this.script.LoadSourceFile( fileName );
    }

    /// <summary>
    /// Loads the given stream into the script.
    /// </summary>
    /// <param name="strm">Stream containing the script source</param>
    public void LoadScript( Stream strm )
    {
      if( this.script == null )
      {
        throw new InvalidOperationException();
      }
      this.script.LoadSource( strm );
    }

    /// <summary>
    /// Compiles the script loaded in the ScriptEngine.
    /// </summary>
    /// <returns>true if successful; otherwise false</returns>
    /// <remarks>
    /// <para>
    /// The <see cref="Syncfusion.Scripting.ScriptingManager.OnCompileError"/>
    /// method is called if an error occurs during compilation.
    /// </para>
    /// </remarks>
    public bool CompileScript()
    {
      bool compiled = false;

      this.CreateScriptEngine();

      if( this.scriptEngine == null )
      {
        throw new InvalidOperationException();
      }

      //if(!this.scriptEngine.IsCompiled && this.script != null)
      if( this.script != null )
      {
        this.scriptEngine.RootMoniker = this.script.RootMoniker;

        this.scriptSite.ClearCompiledState();
        this.scriptEngine.Site = this.scriptSite;
        this.scriptEngine.InitNew();

        this.scriptEngine.RootNamespace = this.script.RootNamespace;
		if((this.applicationBase != string.Empty) && (this.script.Language == ScriptLanguages.VisualBasic))
			this.scriptEngine.SetOption("ApplicationBase", this.applicationBase);

        this.script.LoadScriptEngineItems( this.scriptEngine );

        compiled = this.scriptEngine.Compile();

        if( compiled )
        {
          this.scriptSite.SetCompiledState( this.scriptEngine );
          this.scriptDirty = false;
        }
      }

      return compiled;
    }

    /// <summary>
    /// Runs the script that is compiled in the ScriptEngine.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The script must be loaded and compiled before this method is called.
    /// An error occurs if the ScriptEngine is already running.
    /// </para>
    /// </remarks>
    public void RunScript()
    {
      if( this.scriptEngine == null )
      {
        throw new InvalidOperationException();
      }

	  try
	  {
		this.scriptEngine.Run();
	  }
	  catch(Exception e)
	  {
		Debug.WriteLine(e.Message);
	  }

      string entryPoint = this.Script.EntryPoint;
      if( ( entryPoint != null ) && ( entryPoint.Length > 0 ) )
      {
        this.InvokeScriptMethod( entryPoint );
      }

      string startcall = this.Script.ScriptStartCall;
      if( ( startcall != null ) && ( startcall.Length > 0 ) )
      {
        this.InvokeScriptMethod( startcall );
      }
    }

    /// <summary>
    /// Removes the script engine from the running state and disconnects
    /// automatically bound event handlers.
    /// </summary>
    public void ResetScriptEngine()
    {
      if( this.scriptEngine == null )
      {
        throw new InvalidOperationException();
      }

      if( this.scriptEngine.IsRunning == true )
      {
        string stopcall = this.Script.ScriptStopCall;
        if( ( stopcall != null ) && ( stopcall.Length > 0 ) )
        {
          this.InvokeScriptMethod( stopcall );
        }
      }

      this.scriptEngine.Reset();
    }

    /// <summary>
    /// Closes the script engine and releases all resources. If the script engine
    /// is currently running, the IVsaEngine.Reset method is called first.
    /// </summary>
    public void CloseScriptEngine()
    {
      if( this.scriptEngine == null )
      {
        throw new InvalidOperationException();
      }

      if( this.scriptEngine.IsRunning == true )
      {
        string stopcall = this.Script.ScriptStopCall;
        if( ( stopcall != null ) && ( stopcall.Length > 0 ) )
        {
          this.InvokeScriptMethod( stopcall );
        }
      }

      this.scriptEngine.Close();
    }

    /// <summary>
    /// Removes all event sources, global instances, and compiled code from the site.
    /// </summary>
    public void ClearScriptSite()
    {
      if( this.scriptSite != null )
      {
        this.scriptSite.Clear();
      }
    }

    /// <summary>
    /// Registers and event source with the site.
    /// </summary>
    /// <param name="name">Name of the event source</param>
    /// <param name="obj">Event source object</param>
    public void RegisterEventSource( string name, object obj )
    {
      if( this.scriptSite != null )
      {
        this.scriptSite.RegisterEventSource( name, obj );
      }
    }

    /// <summary>
    /// Registers a global instance with the site.
    /// </summary>
    /// <param name="name">Name of the global instance</param>
    /// <param name="obj">Global instance object</param>
    public void RegisterGlobalInstance( string name, object obj )
    {
      if( this.scriptSite != null )
      {
        this.scriptSite.RegisterGlobalInstance( name, obj );
      }
    }

    /// <summary>
    /// Invokes the specified method in the script.
    /// </summary>
    /// <param name="methodName">Name of method to invoke</param>
    /// <returns>Return value of the method</returns>
    /// <remarks>
    /// <para>
    /// This method throws an InvalidOperationException if the ScriptEngine
    /// or Script is null. The ScriptEngine must be running to call this
    /// method.
    /// </para>
    /// </remarks>
    public object InvokeScriptMethod( string methodName )
    {
      if( this.scriptEngine == null || this.script == null )
      {
        throw new InvalidOperationException();
      }

      return this.script.InvokeMethod( this.scriptEngine, methodName );
    }
    #endregion

    #region Public Events
    /// <summary>
    /// Fired when an error occurs during compilation.
    /// </summary>
    public event VsaErrorEventHandler CompileError;

    /// <summary>
    /// Fired when the script changes
    /// </summary>
    public event ScriptEventHandler ScriptChanged;
    #endregion

    #region Implementation Methods
    /// <summary>
    /// Initializes the ScriptingManager
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method calls the
    /// <see cref="Syncfusion.Scripting.ScriptingManager.CreateScriptSite"/>
    /// and
    /// <see cref="Syncfusion.Scripting.ScriptingManager.CreateScript"/>
    /// methods and attaches the ScriptSite and Script to the ScriptingManager.
    /// If the ScriptingManager already has a ScriptSite attached, then a new
    /// one is not created. The same is true of the Script.
    /// </para>
    /// </remarks>
    protected virtual void Init()
    {
      if( this.scriptSite == null )
      {
        this.scriptSite = this.CreateScriptSite();
      }

      if( this.scriptSite == null )
      {
        throw new InvalidOperationException();
      }

      if( this.script == null )
      {
        this.script = this.CreateScript();
      }

      if( this.script == null )
      {
        throw new InvalidOperationException();
      }

      this.scriptSite.CompileError += new VsaErrorEventHandler( scriptSite_CompileError );
      this.script.Changed += new ScriptEventHandler( script_Changed );
    }

    /// <summary>
    /// Creates a new instance of the ScriptEngine and loads the script into it.
    /// </summary>
    /// <returns>true if successful; otherwise false</returns>
    protected virtual bool CreateScriptEngine()
    {
      bool success = false;

      if( this.scriptEngine != null )
      {
        this.scriptEngine.Close();
        this.scriptEngine = null;
      }

      if( this.script != null )
      {
        this.scriptEngine = ScriptEngineFactory.CreateScriptEngine( this.script.Language );
      }

      return success;
    }

    /// <summary>
    /// Creates the <see cref="Syncfusion.Scripting.ScriptSite"/> to attach to
    /// the ScriptingManager.
    /// </summary>
    /// <returns><see cref="Syncfusion.Scripting.ScriptSite"/> object</returns>
    protected virtual ScriptSite CreateScriptSite()
    {
      return new DefaultScriptSite();
    }

    /// <summary>
    /// Creates the <see cref="Syncfusion.Scripting.Script"/> to attach to
    /// the ScriptingManager.
    /// </summary>
    /// <returns><see cref="Syncfusion.Scripting.Script"/> object</returns>
    protected virtual Script CreateScript()
    {
      return new Script();
    }

    /// <summary>
    /// Called when an error occurs during compilation.
    /// </summary>
    /// <param name="evtArgs">Event arguments</param>
    protected virtual void OnCompileError( VsaErrorEventArgs evtArgs )
    {
      if( this.CompileError != null )
      {
        this.CompileError( this, evtArgs );
      }
    }

    /// <summary>
    /// Called when the script changes.
    /// </summary>
    /// <param name="evtArgs">Event arguments</param>
    protected virtual void OnScriptChanged( ScriptEventArgs evtArgs )
    {
      this.scriptDirty = true;

      if( this.ScriptChanged != null )
      {
        this.ScriptChanged( this, evtArgs );
      }
    }

    private void scriptSite_CompileError( object sender, VsaErrorEventArgs evtArgs )
    {
      this.OnCompileError( evtArgs );
    }

    private void script_Changed( object sender, ScriptEventArgs evtArgs )
    {
      this.OnScriptChanged( evtArgs );
    }

    /// <summary>
    /// For testing.
    /// </summary>
    /// <param name="asm"></param>
    private void SaveAssembly( Assembly asm, string path )
    {
      /*
#if DEBUG
      if( asm == null )
        throw new ArgumentNullException( "asm" );

      if( path == null )
        throw new ArgumentNullException( "path" );

      if( path.Length == 0 )
        throw new ArgumentException( "path - string can not be empty" );

      AppDomain currentDom = Thread.GetDomain();
      AssemblyName myAsmName = new AssemblyName();
      myAsmName.Name = "MyDynamicAssembly";

      AssemblyBuilder myAsmBldr = currentDom.DefineDynamicAssembly(
        myAsmName,
        AssemblyBuilderAccess.RunAndSave );
      

      myAsmBldr.Save( path );
#endif
      */
    }
    #endregion

    #region Fields
    private IVsaEngine scriptEngine = null;
    private ScriptSite scriptSite = null;
    private Script script = null;
    private bool scriptDirty = true;
    #endregion
  }
}