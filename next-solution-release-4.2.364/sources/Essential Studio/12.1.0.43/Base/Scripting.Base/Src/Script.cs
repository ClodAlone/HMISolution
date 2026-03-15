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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Microsoft.Vsa;

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Languages supported by the scripting subsystem.
  /// </summary>
  [ TypeConverter( typeof( ScriptLanguagesConverter ) ) ]
  public enum ScriptLanguages
  {
    /// <summary>
    /// Visual Basic .NET script engine
    /// </summary>
    VisualBasic,

    /// <summary>
    /// JScript .NET script engine
    /// </summary>
    JScript,

    /// <summary>
    /// C# .NET script engine
    /// </summary>
    CSharp
  }

  public class ScriptLanguagesConverter : TypeConverter
  {
    public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
    {
      if( destinationType == typeof( string ) )
      {
        if( value != null )
        {
          ScriptLanguages language = ( ScriptLanguages )value;
          switch( language )
          {
            case ScriptLanguages.CSharp:
              return "CSharp";
            case ScriptLanguages.JScript:
              return "JScript";
            case ScriptLanguages.VisualBasic:
              return "VisualBasic";
          }
        }
      }
      return base.ConvertTo( context, culture, value, destinationType );
    }

    public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
    {
      if( value != null && value.GetType() == typeof( string ) )
      {
        string languagename = ( string )value;
        switch( languagename )
        {
          case "CSharp":
            return ScriptLanguages.CSharp;
          case "JScript":
            return ScriptLanguages.JScript;
          case "VisualBasic":
            return ScriptLanguages.VisualBasic;
        }
      }
      return base.ConvertFrom( context, culture, value );
    }
  }

  public class ScriptLanguageUtilities
  {
    public static string GetFileExtensionFromLanguage( ScriptLanguages language )
    {
      if( language == ScriptLanguages.CSharp )
      {
        return ".cs";
      }
      else if( language == ScriptLanguages.JScript )
      {
        return ".js";
      }
      else
      {
        return ".vb";
      }
    }

    public static ScriptLanguages GetLanguageFromFileExtension( string extension )
    {
      if( extension == ".cs" )
      {
        return ScriptLanguages.CSharp;
      }
      else if( extension == ".js" )
      {
        return ScriptLanguages.JScript;
      }
      else
      {
        return ScriptLanguages.VisualBasic;
      }
    }
  }

  /// <summary>
  /// Encapsulates the source code, references, and global instances of a script.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A script consists of source code, assembly references, global instances, and
  /// event sources. A script also has a name associated with it.
  /// </para>
  /// </remarks>
  [ Serializable ]
  public class Script : ISerializable
  {
    #region Class constants
    /// <summary>
    /// List of assemblies attached automatically to engine on execution
    /// </summary>
    public static readonly string[] DEFAULT_ASSEMBLIES = new string[]
      {
        "System",
        "mscorlib",
        "System.Windows.Forms"
      };

    /// <summary>
    /// Default name of the script.
    /// </summary>
    private const string DEF_NAME = "Script";
    /// <summary>
    /// Default namespace for the script.
    /// </summary>
    private const string DEF_NAMESPACE = "Syncfusion";
    /// <summary>
    /// Delimiter for moniker value constructing.
    /// </summary>
    private const string DEF_DELIMITER = "://";
    /// <summary>
    /// Class keyword.
    /// </summary>
    private const string DEF_CLASS = "class";
    /// <summary>
    /// Class keyword definition for Visual Basic.NET.
    /// </summary>
	private const string DEF_VB_CLASS = "module";
    /// <summary>
    /// Class keyword definition for JScript.NET.
    /// </summary>
    private const string DEF_JS_CLASS = "class";
	/// <summary>
	/// Class keyword defintion for CSharp.
	/// </summary>
    private const string DEF_CS_CLASS = "class";
   /// <summary>
   /// Place holder for class name.
   /// </summary>
    private const string DEF_CLASS_NAME = "<%?ClassName%>";
	/// <summary>
	/// Class header for CSharp.
	/// </summary>
	private const string DEF_CS_CLASS_HEADER = "public class " + DEF_CLASS_NAME;
	/// <summary>
	/// Class footer for CSharp.
	/// </summary>
	private const string DEF_CS_CLASS_FOOTER = "}";
	/// <summary>
	/// Class header for Visual Basic Net.
	/// </summary>
	private const string DEF_VB_CLASS_HEADER = "module " + DEF_CLASS_NAME;
	/// <summary>
	/// Class footer for Visual Basic Net.
	/// </summary>
	private const string DEF_VB_CLASS_FOOTER = "end module";
	/// <summary>
	/// Class header for Java Script Net.
	/// </summary>
	private const string DEF_JS_CLASS_HEADER = "class " + DEF_CLASS_NAME;
	/// <summary>
	/// Class footer for Java Script Net.
	/// </summary>
	private const string DEF_JS_CLASS_FOOTER = DEF_CS_CLASS_FOOTER;    
    /// <summary>
    /// Namespace keyword.
    /// </summary>
    private const string DEF_NAMESPACE_KEY = "namespace";
    /// <summary>
    /// Delimiters between class keyword and name of the class.
    /// </summary>
    private readonly char[] DEF_WORD_DELIMITERS = " \t\r\n".ToCharArray();
    /// <summary>
    /// Whitespace keyword.
    /// </summary>
    private const char DEF_WHITESPACE = ' ';
    /// <summary>
    /// Open brace keyword.
    /// </summary>
    private const char DEF_BRACE_OPEN = '{';
    /// <summary>
    /// Close brace keyword.
    /// </summary>
    private const char DEF_BRACE_CLOSE = '}';
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    public Script()
    {
      foreach( string assembly in Script.DEFAULT_ASSEMBLIES )
      {
        this.assemblyReferences.Add( new AssemblyDescriptor( assembly, String.Empty, String.Empty ) );
      }
    }

    /// <summary>
    /// Construct a script with the given source code.
    /// </summary>
    /// <param name="sourceText">Source code to assign to the script</param>
    public Script( string sourceText )
      : base()
    {
      this.SourceText = sourceText;
    }

    /// <summary>
    /// Serialization constructor for script objects.
    /// </summary>
    /// <param name="info">Serialization state information</param>
    /// <param name="context">Streaming context information</param>
    protected Script( SerializationInfo info, StreamingContext context )
    {
      this.name = info.GetString( "name" );
      this.sourceText = info.GetString( "sourceText" );
      this.rootMoniker = info.GetString( "rootMoniker" );
      this.rootNamespace = info.GetString( "rootNamespace" );
      this.assemblyReferences = ( AssemblyDescriptorCollection )info.GetValue( "assemblyReferences", typeof( AssemblyDescriptorCollection ) );
      this.globalInstances = ( ScriptObjectCollection )info.GetValue( "globalInstances", typeof( ScriptObjectCollection ) );
      this.eventSources = ( ScriptObjectCollection )info.GetValue( "eventSources", typeof( ScriptObjectCollection ) );
      this.scriptLang = ( ScriptLanguages )info.GetValue( "scriptLang", typeof( ScriptLanguages ) );
      this.entryPoint = info.GetString( "entryPoint" );
      this.scriptStartCall = info.GetString("StartCall");
      this.scriptStopCall = info.GetString("StopCall");
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Name of the script.
    /// </summary>
    /// <remarks>Name of the script must be the same as name of the class in the script source.</remarks>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        if( name != value )
        {
          this.name = value;
          ChangeMoniker();
        }
      }
    }

    /// <summary>
    /// Names of assemblies referenced by the script.
    /// </summary>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public AssemblyDescriptorCollection AssemblyReferences
    {
      get
      {
        return this.assemblyReferences;
      }
    }

    /// <summary>
    /// Names of global instances declared by the script.
    /// </summary>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public ScriptObjectCollection GlobalInstances
    {
      get
      {
        return this.globalInstances;
      }
    }

    /// <summary>
    /// Names of event sources referenced by the script.
    /// </summary>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public ScriptObjectCollection EventSources
    {
      get
      {
        return this.eventSources;
      }
    }

    /// <summary>
    /// Source code for the script.
    /// </summary>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public string SourceText
    {
      get
      {
        return this.sourceText;
      }
      set
      {
        if( !this.sourceText.Equals( value ) )
        {
          this.sourceText = value;
          this.SetNameFromSourceText(value);
          this.OnSourceTextChanged(new ScriptEventArgs(this));
        }
      }
    }

    /// <summary>
    /// Root moniker for the script.
    /// </summary>
    /// <remarks>The moniker, or root moniker, is the unique name by which
    /// a script engine is identified. 
    /// However, an engine may throw a RootMonikerInUse
    /// exception if it detects that the moniker you are attempting to assign
    /// it is already assigned to another script engine. 
    /// The root moniker cannot exceed 256 characters in length.
    /// By default this property is constructed from RootNamespace and 
    /// Name property. If you use several scripts on the same host, 
    /// be sure that RootNamespace or Name properties are different.</remarks>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public string RootMoniker
    {
      get
      {
        return this.rootMoniker;
      }
      set
      {
        if( !this.rootMoniker.Equals( value ) )
        {
          this.rootMoniker = value;
          monikerWasSet = true;
          this.OnChanged( new ScriptEventArgs( this ) );
        }
      }
    }

    /// <summary>
    /// Root namespace for declarations in the script.
    /// </summary>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public string RootNamespace
    {
      get
      {
        return this.rootNamespace;
      }
      set
      {
        if( !this.rootNamespace.Equals( value ) )
        {
          this.rootNamespace = value;
          ChangeMoniker();
          this.OnChanged( new ScriptEventArgs( this ) );
        }
      }
    }

    /// <summary>
    /// Script language that the source code is written in.
    /// </summary>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public ScriptLanguages Language
    {
      get
      {
        return this.scriptLang;
      }
      set
      {
        if( this.scriptLang != value )
        {
          this.scriptLang = value;
          this.OnChanged( new ScriptEventArgs( this ) );
        }
      }
    }

    /// <summary>
    /// The starting method that will be called when the script is run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this property is null, then no method is called when the script
    /// is started. It is null by default.
    /// </para>
    /// </remarks>
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public string EntryPoint
    {
      get
      {
        return this.entryPoint;
      }
      set
      {
        if( this.entryPoint != value )
        {
          this.entryPoint = value;
          this.OnChanged( new ScriptEventArgs( this ) );
        }
      }
    }

    /// <summary>
    /// The ScriptStartCall method is invoked right after the script is started.
    /// </summary>				
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public string ScriptStartCall
    {
      get
      {
        return this.scriptStartCall;
      }
      set
      {
        if( this.scriptStartCall != value )
        {
          this.scriptStartCall = value;
          this.OnChanged( new ScriptEventArgs( this ) );
        }
      }
    }

    /// <summary>
    /// The ScriptStopCall method is invoked just before the script is stopped.
    /// </summary>		
    [
      Browsable( true ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
      ]
    public string ScriptStopCall
    {
      get
      {
        return this.scriptStopCall;
      }
      set
      {
        if( this.scriptStopCall != value )
        {
          this.scriptStopCall = value;
          this.OnChanged( new ScriptEventArgs( this ) );
        }
      }
    }

    /// <summary>
    /// Flag indicating if firing of events is enabled.
    /// </summary>
    [
      Browsable( false ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
      ]
    public bool EventsEnabled
    {
      get
      {
        return this.eventsEnabled;
      }
      set
      {
        this.eventsEnabled = value;
      }
    }

    /// <summary>
    /// Gets or sets if parsing of source text and assigning of Name property
    /// by class name is required or not.
    /// </summary>
    [
      Browsable( false ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
      ]
    public bool SetNameFromSource
    {
      get
      {
        return setName;
      }
      set
      {
        if( setName != value )
        {
          setName = value;
        }
      }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Loads the contents of the specified file into the script.
    /// </summary>
    /// <param name="fileName">Name of file containing the script source</param>
    public void LoadSourceFile( string fileName )
    {
      if( fileName == null )
      {
        throw new ArgumentNullException( "fileName" );
      }

      if( fileName.Length == 0 )
      {
        throw new ArgumentException( "fileName - string can not be empty" );
      }

      string full = Path.GetFullPath( fileName );
      string dir = Path.GetDirectoryName( full );

      // create directory for output file if it does not exists yet
      if( !Directory.Exists( dir ) )
      {
        Directory.CreateDirectory( dir );
      }

      /// TODO: if file with the same name exists and it ReadOnly set,
      /// then throw exception
      if( File.Exists( full ) )
      {
        FileAttributes attrib = File.GetAttributes( full );

        //if( ( attrib & FileAttributes.ReadOnly ) != 0 )
        //{
        //  // RaiseReadOnlyFileEvent( tmpFullPath );
        //  throw new ArgumentException( "File attributes set to read-only state. File Name: " + full );
        //}

        using( StreamReader scriptReader = new StreamReader( fileName ) )
        {
          this.SourceText = scriptReader.ReadToEnd();
        }
      }
    }

    /// <summary>
    /// Loads the given stream into the script.
    /// </summary>
    /// <param name="strm">Stream containing the script source</param>
    public void LoadSource( Stream strm )
    {
      StreamReader scriptReader = new StreamReader( strm );
      this.SourceText = scriptReader.ReadToEnd();
      scriptReader.Close();
    }

    /// <summary>
    /// Saves the script source text into the specified file.
    /// </summary>
    /// <param name="fileName">Name of file to save source into</param>
    public void SaveSourceFile( string fileName )
    {
      if( fileName == null )
      {
        throw new ArgumentNullException( "fileName" );
      }

      if( fileName.Length == 0 )
      {
        throw new ArgumentException( "fileName - string can not be empty" );
      }

      using( FileStream fs = new FileStream( fileName, FileMode.Create, FileAccess.Write, FileShare.None ) )
      {
        byte[] data = Encoding.UTF8.GetBytes( this.SourceText );
        fs.Write( data, 0, data.Length );
      }
    }

    /// <summary>
    /// Adds the specified assembly reference to the script.
    /// </summary>
    /// <param name="descriptor">The AssemblyDescriptor representing the assembly.</param>
    /// <returns>
    /// true if successfully added; false if the assembly reference already exists
    /// in the script
    /// </returns>
    public bool AddReference( AssemblyDescriptor descriptor )
    {
      bool success = false;

      if( !this.ContainsReference( descriptor ) )
      {
        this.assemblyReferences.Add( descriptor );
        this.OnChanged( new ScriptEventArgs( this ) );
        success = true;
      }

      return success;
    }

    /// <summary>
    /// Determines if the script contains the specified assembly reference.
    /// </summary>
    /// <param name="descriptor">The AssemblyDescriptor representing the assembly.</param>
    /// <returns>
    /// true if script contains a reference to the assembly; otherwise false
    /// </returns>
    public bool ContainsReference( AssemblyDescriptor descriptor )
    {
      return this.assemblyReferences.Contains( descriptor );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assemblyReferences"></param>
    public void GetReferences( AssemblyDescriptorCollection assemblyreferences )
    {
      foreach( AssemblyDescriptor descriptor in this.assemblyReferences )
      {
        assemblyreferences.Add( descriptor );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assemblyReferences"></param>
    public void PutReferences( AssemblyDescriptorCollection assemblyreferences )
    {
      this.assemblyReferences.Clear();
      foreach( AssemblyDescriptor descriptor in assemblyreferences )
      {
        this.assemblyReferences.Add( descriptor );
      }
    }

    /// <summary>
    /// Adds an event source to the script.
    /// </summary>
    /// <param name="name">Name of event source to add</param>
    /// <param name="typeName">Type of event source as a string</param>
    /// <returns>true if added successfully; false if it already exists</returns>
    public bool AddEventSource( string name, string typeName )
    {
      bool success = false;

      if( !this.ContainsEventSource( name ) )
      {
        ScriptObject evtSrc = new ScriptObject( name, typeName );
        this.eventSources.Add( evtSrc );
        this.OnChanged( new ScriptEventArgs( this ) );
        success = true;
      }

      return success;
    }

    /// <summary>
    /// Indicates if the specified event source exists in the script.
    /// </summary>
    /// <param name="name">Name of global instance to search for</param>
    /// <returns></returns>
    public bool ContainsEventSource( string name )
    {
      foreach( ScriptObject curEvtSrc in this.eventSources )
      {
        if( curEvtSrc.Name.Equals( name ) )
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventSources"></param>
    public void GetEventSources( ScriptObjectCollection eventSources )
    {
      foreach( ScriptObject evtSource in this.eventSources )
      {
        eventSources.Add( new ScriptObject( evtSource ) );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventSources"></param>
    public void PutEventSources( ScriptObjectCollection eventSources )
    {
      this.eventSources.Clear();

      foreach( ScriptObject evtSource in eventSources )
      {
        this.eventSources.Add( new ScriptObject( evtSource ) );
      }
    }

    /// <summary>
    /// Adds a global instance to the script.
    /// </summary>
    /// <param name="name">Name of global instance to add</param>
    /// <param name="typeName">Type of global instance as a string</param>
    /// <returns>true if added successfully; false if it already exists</returns>
    public bool AddGlobalInstance( string name, string typeName )
    {
      bool success = false;

      if( !this.ContainsGlobalInstance( name ) )
      {
        ScriptObject globInst = new ScriptObject( name, typeName );
        this.globalInstances.Add( globInst );
        this.OnChanged( new ScriptEventArgs( this ) );
        success = true;
      }

      return success;
    }

    /// <summary>
    /// Indicates if the specified global instance exists in the script.
    /// </summary>
    /// <param name="name">Name of global instance to search for</param>
    /// <returns></returns>
    public bool ContainsGlobalInstance( string name )
    {
      foreach( ScriptObject curGlobalInstance in this.globalInstances )
      {
        if( curGlobalInstance.Name.Equals( name ) )
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="globalInstances"></param>
    public void GetGlobalInstances( ScriptObjectCollection globalInstances )
    {
      foreach( ScriptObject globalInst in this.globalInstances )
      {
        globalInstances.Add( new ScriptObject( globalInst ) );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="globalInstances"></param>
    public void PutGlobalInstances( ScriptObjectCollection globalInstances )
    {
      this.globalInstances.Clear();

      foreach( ScriptObject globalInst in globalInstances )
      {
        this.globalInstances.Add( new ScriptObject( globalInst ) );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scriptEngine"></param>
    /// <returns></returns>
    public bool LoadScriptEngineItems( IVsaEngine scriptengine )
    {
      if( scriptengine == null )
      {
        throw new ArgumentNullException();
      }

      this.LoadReferences( scriptengine );
      this.LoadGlobalInstances( scriptengine );

      IVsaCodeItem codemodule = this.LoadSource( scriptengine );
      if( this.scriptLang == ScriptLanguages.VisualBasic )
      {
        if( codemodule != null )
        {
          this.LoadEventSources( codemodule );
        }
      }
      else // JScript does not support implicit event binding
      {
        this.LoadEventSourcesAsGlobalInstances( scriptengine );
      }

      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scriptEngine"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public object InvokeMethod( IVsaEngine scriptEngine, string methodName )
    {
      object rtnValue = null;

      if( scriptEngine == null )
      {
        throw new ArgumentNullException();
      }

      if( !scriptEngine.IsRunning )
      {
        throw new InvalidOperationException();
      }

      Assembly scriptAssembly = scriptEngine.Assembly;

      if( scriptAssembly != null )
      {
        Type scriptType = scriptAssembly.GetType( this.rootNamespace + "." + this.Name );
        if( scriptType != null )
        {
          MethodInfo method = scriptType.GetMethod( methodName );
          if( method != null )
          {
            rtnValue = method.Invoke(this, null);
          }
        }
      }

      return rtnValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return this.Name;
    }
    #endregion

    #region Public Events
    /// <summary>
    /// Fired when the script is changed.
    /// </summary>
    public event ScriptEventHandler Changed;
    #endregion

    #region ISerializable Interface
    void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
    {
      this.GetObjectData( info, context );
    }
    #endregion

    #region Implementation Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="scriptEngine"></param>
    /// <returns></returns>
    private IVsaCodeItem LoadSource( IVsaEngine scriptEngine )
    {
      IVsaCodeItem codeItem = null;
      IVsaItems scriptItems = null;

      if( scriptEngine != null )
      {
        scriptItems = scriptEngine.Items;
        codeItem = ( IVsaCodeItem )scriptItems.CreateItem( this.Name, VsaItemType.Code, VsaItemFlag.None );
        codeItem.SourceText = this.SourceText;
      }

      return codeItem;
    }

    /// <summary>
    /// Load assembly references required by the script into the given
    /// scripting engine.
    /// </summary>
    /// <param name="scriptEngine">Script engine to load</param>
    private void LoadReferences( IVsaEngine scriptEngine )
    {
      // Load the referenced assemblies

      IVsaReferenceItem refItem;
      IVsaItems scriptItems = scriptEngine.Items;

      // Add reference to system.dll
      refItem = ( IVsaReferenceItem )scriptItems.CreateItem( "system.dll", VsaItemType.Reference, VsaItemFlag.None );
      refItem.AssemblyName = "system.dll";

      // Add reference to mscorlib.dll
      refItem = ( IVsaReferenceItem )scriptItems.CreateItem( "mscorlib.dll", VsaItemType.Reference, VsaItemFlag.None );
      refItem.AssemblyName = "mscorlib.dll";

      // Add reference to System.Windows.Forms
      refItem = ( IVsaReferenceItem )scriptItems.CreateItem( "System.Windows.Forms.dll", VsaItemType.Reference, VsaItemFlag.None );
      refItem.AssemblyName = "System.Windows.Forms.dll";

      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach( AssemblyDescriptor descriptor in this.assemblyReferences )
      {
        if( Array.IndexOf( Script.DEFAULT_ASSEMBLIES, descriptor.DisplayName ) >= 0 )
        {
          continue;
        }

        Assembly assembly = null;
        foreach( Assembly curassembly in assemblies )
        {
          if( curassembly.GetName().Name.Equals( descriptor.DisplayName ) )
          {
            assembly = curassembly;
            break;
          }
        }

        if( assembly == null )
        {
          if( descriptor.FullName != String.Empty )
          {
            assembly = Assembly.Load( descriptor.FullName );
          }
          else if( descriptor.Location != String.Empty )
          {
            assembly = Assembly.LoadFrom( descriptor.Location );
          }
          else
          {
            assembly = Assembly.LoadWithPartialName( descriptor.DisplayName );
          }
        }

        if( assembly != null )
        {
          string itemname = assembly.Location;
          if(descriptor.Location != String.Empty)
          {
            itemname = descriptor.Location;
          }
          else if((assembly.GlobalAssemblyCache == true) && (this.scriptLang == ScriptLanguages.VisualBasic))
          {
			  // Visual Basic depends on the 'ApplicationBase' ScriptEngine option for the reference assemblies
			  itemname = itemname.Substring( itemname.LastIndexOf( '\\' ) + 1 );
          }

          refItem = ( IVsaReferenceItem )scriptItems.CreateItem( itemname, VsaItemType.Reference, VsaItemFlag.None );
          refItem.AssemblyName = itemname;
        }
      }
    }

    /// <summary>
    /// Load the event sources required by the script into the given
    /// scripting engine code item.
    /// </summary>
    /// <param name="codeModule">Code module contained by a scripting engine</param>
    private void LoadEventSources( IVsaCodeItem codeModule )
    {
      foreach(ScriptObject evtSrc in this.eventSources)
      {
        codeModule.AddEventSource(evtSrc.Name, evtSrc.TypeName);
      }
    }

    /// <summary>
    /// Load the global instances required by the script into the given
    /// scripting engine.
    /// </summary>
    /// <param name="scriptEngine">Scripting engine to load.</param>
    private void LoadGlobalInstances( IVsaEngine scriptEngine )
    {
      foreach( ScriptObject globalInst in this.globalInstances )
      {
        IVsaGlobalItem globItem = scriptEngine.Items.CreateItem( globalInst.Name, VsaItemType.AppGlobal, VsaItemFlag.None ) as IVsaGlobalItem;
        if( globItem != null )
        {
          globItem.TypeString = globalInst.TypeName;
        }
      }
    }

    /// <summary>
    /// Loads the event sources required by the script as global objects. Used by languages that do not support implicit event-binding.
    /// </summary>
    /// <param name="scriptengine">Scripting engine to load.</param>
    private void LoadEventSourcesAsGlobalInstances( IVsaEngine scriptengine )
    {
      foreach( ScriptObject evntsource in this.eventSources )
      {
        /// NOTE: if language is JScript or C# and global variable with such
        /// name also exists, code below was already executed in method LoadGlobalInstances
        /// and doesn't to be executed again, otherwise error occurs.
        if( NeedToAdd( evntsource.Name ) )
        {
          IVsaGlobalItem globItem = scriptengine.Items.CreateItem( evntsource.Name, VsaItemType.AppGlobal, VsaItemFlag.None ) as IVsaGlobalItem;
          if( globItem != null )
          {
            globItem.TypeString = evntsource.TypeName;
          }
        }
      }
    }

    /// <summary>
    /// Called when the script is changed.
    /// </summary>
    /// <param name="evtArgs"></param>
    protected virtual void OnChanged( ScriptEventArgs evtArgs )
    {
      if( this.Changed != null && this.eventsEnabled )
      {
        this.Changed( this, evtArgs );
      }
    }

    /// <summary>
    /// Raises when source text has been changed.
    /// </summary>
    /// <param name="evtArgs"></param>
    protected virtual void OnSourceTextChanged( ScriptEventArgs evtArgs )
    {
      OnChanged( evtArgs );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected virtual void GetObjectData( SerializationInfo info, StreamingContext context )
    {
      info.AddValue( "name", this.name );
      info.AddValue( "sourceText", this.sourceText );
      info.AddValue( "rootMoniker", this.rootMoniker );
      info.AddValue( "rootNamespace", this.rootNamespace );
      info.AddValue( "assemblyReferences", this.assemblyReferences );
      info.AddValue( "globalInstances", this.globalInstances );
      info.AddValue( "eventSources", this.eventSources );
      info.AddValue( "scriptLang", this.scriptLang );
      info.AddValue( "entryPoint", this.entryPoint );
      info.AddValue( "StartCall", this.scriptStartCall );
      info.AddValue( "StopCall", this.scriptStopCall );
    }

    /// <summary>
    /// Checks if variable with such name can be added as Event Source.
    /// </summary>
    /// <param name="varName">Name of variable.</param>
    /// <returns>TRUE - if can be added, FALSE - otherwise.</returns>
    private bool NeedToAdd( string varName )
    {
      if( this.Language == ScriptLanguages.VisualBasic )
      {
        return true;
      }

      bool bNeed = true;

      foreach( ScriptObject obj in this.globalInstances )
      {
        if( string.Compare( obj.Name, varName, false,
                            CultureInfo.InvariantCulture ) == 0 )
        {
          bNeed = false;
          break;
        }
      }

      return bNeed;
    }

    /// <summary>
    /// Changes rootmoniker according to Name or RootNamespace changing.
    /// </summary>
    private void ChangeMoniker()
    {
      if( monikerWasSet )
      {
        return;
      }

      rootMoniker = rootNamespace + DEF_DELIMITER + name;
    }

    /// <summary>
    /// Sets name of the script according to class name in the source text.
    /// </summary>
    /// <param name="source">Text source of the script.</param>
    /// <remarks>If language is C#, also defines Root namespace.</remarks>
    private void SetNameFromSourceText( string source )
    {
      this.SetClassName(source);

      // We need to add the namespace keyword if the source language in C#
      if(!SetNamespace(source) && this.RootNamespace != null &&
        this.RootNamespace.Length > 0 &&
        this.Language == ScriptLanguages.CSharp)
      {
        // If the code has any 'using' directives, insert the namespace block after the using directives
	    String namespacestart = String.Concat(DEF_NAMESPACE_KEY, DEF_WHITESPACE, this.RootNamespace, Environment.NewLine, DEF_BRACE_OPEN, Environment.NewLine);
	    String namespaceend = String.Concat(Environment.NewLine, DEF_BRACE_CLOSE);
	    
		string classheader = DEF_VB_CLASS_HEADER;
	    if(this.scriptLang == ScriptLanguages.CSharp)
			classheader = DEF_CS_CLASS_HEADER;
	    else if(this.scriptLang == ScriptLanguages.JScript)
			classheader = DEF_JS_CLASS_HEADER;
	    classheader = classheader.Replace(DEF_CLASS_NAME, this.name);

		int classstart = this.sourceText.IndexOf(classheader);
		if(classstart >= 0)
		  this.sourceText = this.sourceText.Insert(classstart, namespacestart) + namespaceend;
      }
    }

    /// <summary>
    /// Defines name of the class from soure text.
    /// </summary>
    /// <param name="source">Source text of the script.</param>
    private void SetClassName( string source )
    {
      /// Don't set name for the script if user doesn't want to change it
      /// or source text is empty or name property was already changed by user.
      if( !this.SetNameFromSource || source == null ||
        source.Length == 0 || this.Name != DEF_NAME )
      {
        return;
      }

      string lowerSource = source.ToLower(CultureInfo.InvariantCulture);
	  string classname = DEF_VB_CLASS;
	  if(this.scriptLang == ScriptLanguages.JScript)
		  classname = DEF_JS_CLASS;
	  else if(this.scriptLang == ScriptLanguages.CSharp)
		  classname = DEF_CS_CLASS;
      int classIndex = lowerSource.IndexOf(classname);

      if(classIndex < 0)
      {
        return;
      }

      string rightPart = source.Substring(classIndex + classname.Length);
      rightPart = rightPart.TrimStart(DEF_WORD_DELIMITERS);

      string[] words = rightPart.Split(DEF_WORD_DELIMITERS);

      if(words.Length == 0)
      {
        return;
      }

   	  string name = words[0].TrimEnd(new char[]{DEF_WHITESPACE, DEF_BRACE_OPEN});
      if(name.Length > 0)
      {
        this.Name = name;
      }
    }

    /// <summary>
    /// If language is C#, defines root namespace.
    /// </summary>
    /// <param name="source">Source text.</param>
    /// <returns>TRUE - if namespace recognized and defined or nothing
    /// namespace is clear, FALSE - if namespace is needed to add
    /// to source text. </returns>
    private bool SetNamespace( string source )
    {
      /// Don't search namespace if language is not C#.
      if( this.Language != ScriptLanguages.CSharp )
      {
        return true;
      }

      /// don't set namespace for the script if user doesn't want to change it
      /// or source text is empty or namespace property was already changed by user.
      if( !this.SetNameFromSource || source == null ||
        source.Length == 0 || this.RootNamespace != DEF_NAMESPACE )
      {
        return true;
      }

      int classIndex = source.IndexOf( DEF_NAMESPACE_KEY );

      // namespace keyword was not found and required to be added.
      if( classIndex < 0 )
      {
        return false;
      }

      string rightPart = source.Substring( classIndex +
        DEF_NAMESPACE_KEY.Length );
      rightPart = rightPart.TrimStart( DEF_WORD_DELIMITERS );

      string[] words = rightPart.Split( DEF_WORD_DELIMITERS );

      if( words.Length == 0 )
      {
        return true;
      }

      string name = words[ 0 ];

      if( name.Length > 0 )
      {
        this.RootNamespace = name;
      }

      return true;
    }
    #endregion

    #region Fields
    private string name = DEF_NAME;
    private string sourceText = string.Empty;
    private string rootNamespace = DEF_NAMESPACE;
    private string rootMoniker = DEF_NAMESPACE + DEF_DELIMITER + DEF_NAME; //  "syncfusion://script";
    private AssemblyDescriptorCollection assemblyReferences = new AssemblyDescriptorCollection();
    private ScriptObjectCollection globalInstances = new ScriptObjectCollection();
    private ScriptObjectCollection eventSources = new ScriptObjectCollection();
    private ScriptLanguages scriptLang = ScriptLanguages.VisualBasic;
    private string entryPoint = null;
    private string scriptStartCall = "OnScriptStart";
    private string scriptStopCall = "OnScriptStop";
    private bool eventsEnabled = true;

    /// <summary>
    /// Indicates if rootmoniker property was changed by user.
    /// </summary>
    private bool monikerWasSet;

    /// <summary>
    /// Indicates parse source text of the script and set name of 
    /// the script according to class name or not.
    /// </summary>
    private bool setName = true;
    #endregion
  }
}