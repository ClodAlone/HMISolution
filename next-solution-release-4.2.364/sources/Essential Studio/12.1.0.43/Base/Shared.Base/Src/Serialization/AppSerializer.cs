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
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters;
using Microsoft.Win32;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Security.Policy;

namespace Syncfusion.Runtime.Serialization
{	
	/// <summary>
	/// Defines constants for specifying the persistence medium to be used for serialization.
	/// </summary>
	/// <remarks>
	/// The SerializeMode enumeration defines constants for specifying the persistence medium.
	/// This enum is used by the <see cref="AppStateSerializer"/> class as well as by several other 
	/// Syncfusion components.
	/// </remarks>	
	public enum SerializeMode
	{
		/// <summary>
		/// The IsolatedStorage is used for reading / writing the state information.
		/// </summary>
		IsolatedStorage = 0,
		/// <summary>
		/// A Binary file is used for reading / writing the state information.
		/// </summary>
		BinaryFile = 1,
		/// <summary>
		/// A .XML file is used for reading / writing the state information.
		/// </summary>
		XMLFile,
		/// <summary>
		/// The Win32 WindowsRegistry is used for reading / writing the state information.
		/// </summary>
		WindowsRegistry,
		/// <summary>
		/// An externally provided Stream is used for reading / writing the state information in an XML format.
		/// </summary>
		XMLFmtStream,
		/// <summary>
		/// An externally provided Stream is used for reading / writing the state information in a binary format.
		/// </summary>
		BinaryFmtStream
	}
	

	/// <summary>
	/// Provides a mechanism for coordinating the serialization behavior of multiple components.
	/// </summary>
	/// <remarks>
	/// <para>The AppStateSerializer class is a serialization utility that allows multiple components
	/// in an application to access a common disk I/O medium for state persistence. Using
	/// the same storage medium for persisting the state information across components, without overtly
	/// tying them together, helps avoid the file clutter that is bound to occur by components
	/// using distinct files. Though primarily developed for use by Syncfusion products, the
	/// AppStateSerializer is generic enough to be availed of by other components as well. </para>
	/// <para>
	/// The AppStateSerializer supports serializing into the system's Isolated Storage, Windows Registry, an XML file, 
	/// a binary file or to an externally provided Stream. Take a look at the <see cref="SerializeMode"/> enumeration for more information on 
	/// these different supported modes.
	/// </para>
	/// <para>
	/// To use the services of this class, you can create a new instance or use the global Singleton instance.
	/// These two usage patterns are explained below:
	/// </para>
	/// <para>
	/// <b>1) Using the Singleton:</b> The AppStateSerializer class provides you a singleton instance
	/// (through the <see cref="AppStateSerializer.GetSingleton"/>) using which you can persist all your
	/// app. info into a single medium. This singleton, by default, is configured to persist in the
	/// Isolated Storage (with the scope IsolatedStorageScope.Assembly|IsolatedStorageScope.Domain|IsolatedStorageScope.User). 
	/// This usage pattern is akin to creating an instance of this class and using the same instance to persist
	/// all your app information. But, note that this Singleton is also used by the Controls and Components in
	/// Essential Tools to persist their information. The default Storage medium of this Singleton instance
	/// can also be customized using the static <see cref="InitializeSingleton(SerializeMode, Object)"/> method. In short, use
	/// the Singleton whenever you want all your persistence information to be stored in a single medium (along
	/// with the persistence information of the Controls and Components in Essential Tools).
	/// </para>
	/// <para>
	/// <b>2) Using an instance:</b> As an alternative you could create a custom instance
	/// of this class, configuring it to use one of the above storage mediums and persist
	/// one or more information into it. You can use this in tandem with the above Singleton instance
	/// if you wish. Make sure to call <see cref="PersistNow"/> method when you are done writing into the serializer.
	/// </para>
	/// <para>
	/// In both the above cases use the
	/// method's <see cref="AppStateSerializer.SerializeObject(string, object)"/> and <see cref="AppStateSerializer.DeserializeObject"/>
	/// to persist or depersist from the storage medium set for that instance.
	/// </para>
	/// <para>
	/// Note that the AppStateSerializer class uses "Simple" type names (not strongly typed) to 
	/// serialize types. This is necessary to enable usage of persisted information across different
	/// but compatible versions of an assembly. This will however cause the deserialization
	/// process to convert the serialized data to the type from the latest version of the assembly installed in the GAC,
	/// instead of the version that your app is linking to. You can overcome this by
	/// using the <see cref="SetBindingInfo"/> method.
	/// </para>
	/// </remarks>
 	/// <example>
 	/// <para>
 	/// Serializing using an instance:
	/// <code lang="C#">
	/// // To Save
	/// AppStateSerializer serializer = new AppStateSerializer(SerializeMode.XMLFile, "myfile");
	/// serializer.SerializeObject("MyLabel", mydata);
	/// serializer.PersistNow();
	/// // To Load
	/// AppStateSerializer serializer = new AppStateSerializer(SerializeMode.XMLFile, "myfile");
	/// object loadedObj = serializer.DeserializeObject("MyLabel");
	/// if(loadedObj != null &amp;&amp; loadedObj is MyData)
	/// {
	/// 	MyData myData = (MyData)loadedObj;
	/// }
	/// </code>
	/// <code lang="VB">
	/// ' To Save
	/// Dim serializer As New AppStateSerializer(SerializeMode.XMLFile, "myfile")
	/// serializer.SerializeObject("MyLabel", mydata)
	/// serializer.PersistNow()
	/// ' To Load
	/// Dim serializer As New AppStateSerializer(SerializeMode.XMLFile, "myfile")
	/// Dim loadedObj As Object = serializer.DeserializeObject("MyLabel")
	/// If TypeOf loadedObj Is MyData Then
	///		Dim myData As MyData = CType(loadedObj, MyData)
	/// End If
	/// </code>
	/// </para>
	/// <para>
	/// Serializing using Singleton:
	/// <code lang="C#">
	/// // To Save
	/// AppStateSerializer.GetSingleton().SerializeObject("MyLabel", mydata, true);
	/// // To Load
	/// object loadedObj = AppStateSerializer.GetSingleton().DeserializeObject("MyLabel");
	/// </code>
	/// <code lang="VB">
	/// ' To Save
	/// AppStateSerializer.GetSingleton().SerializeObject("MyLabel", mydata, true)
	/// ' To Load
	/// Dim loadedObj As Object = AppStateSerializer.GetSingleton().DeserializeObject("MyLabel")
	/// </code>
	/// </para>
	/// </example>
	public sealed class AppStateSerializer
	{
		private static AppStateSerializer objInstance = null;

		private const String strDefault = "SyncfusionToolsStateInfo";
		private const string strApplicationVerisonName = "ApplicationVersion";
		private const IsolatedStorageScope issDefault = IsolatedStorageScope.Assembly|IsolatedStorageScope.Domain|IsolatedStorageScope.User;
		private bool enabled = true;

		private SerializeMode serMode = SerializeMode.IsolatedStorage;
		private Object serPath = AppStateSerializer.strDefault;
		private IsolatedStorageScope isoScope = issDefault;

		private Hashtable htSerialize = new Hashtable();
		private WeakReference weakRef = null;
		private static CustomSerializationBinder customBinder = new CustomSerializationBinder();

        public static bool AppendFileExtension = true;
        private static bool shouldThrowException = false;

        /// <summary>
        /// Gets or sets a bool value to indicate whether the exception should throw while reading the file from stream or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it should throw the exception to handle it while reading the file stream; otherwise, <c>false</c>.
        /// </value>
        public static bool ShouldThrowException
        {
            get
            {
                return AppStateSerializer.shouldThrowException;
            }
            set
            {
                AppStateSerializer.shouldThrowException = value;
            }
        }
		static AppStateSerializer()
		{			
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public static CustomSerializationBinder CustomBinder
		{
			get{return customBinder;} 
		}

		/// <summary>
		/// Returns the persistence mode set for the <see cref="AppStateSerializer"/>.
		/// </summary>
		/// <value>A <see cref="SerializeMode"/> value.</value>
		public SerializeMode SerializationMode
		{
			get { return this.serMode; }			
		}

		/// <summary>
		/// Returns the persistence path set for the <see cref="AppStateSerializer"/>.
		/// </summary>
		/// <remarks>
		/// The persistence path is contextual and is interpreted based on the <see cref="AppStateSerializer.SerializationMode"/> property value.
		/// </remarks>
		/// <value>An object of type varying based on the the SerializationMode.</value>
		public object SerializationPath
		{
			get { return this.serPath; }			
		}

		/// <summary>
		/// Returns the IsolatedStorageScope used by the <see cref="AppStateSerializer"/>.
		/// </summary>
		/// <value>An <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope"/> value.</value>
		/// <remarks>
		/// This property is used only if the mode is set to SerializeMode.IsolatedStorage.
		/// </remarks>
		public IsolatedStorageScope IsoStorageScope
		{
			get { return this.isoScope; }	
		}

		/// <summary>
		/// Indicates whether serialization and deserialization is enabled.
		/// </summary>
		/// <value>True to enable serialization; false otherwise. This property is set to True by default.</value>
		/// <remarks>
		/// <para>This property allows you to temporarily enable / disable serialization.
		/// </para>
		/// </remarks>
		public bool Enabled
		{
			get{ return enabled;}
			set
			{
				if(enabled != value)
					enabled = value;
			}
		}

		/// <summary>
		/// Returns the version of the application (got through the Application.ProductVersion property)
		/// whose state has now been deserialized.
		/// </summary>
		/// <value>The version as string if available. String.Empty otherwise.</value>
		/// <remarks>This gives you some information about the state of the deserialized information -
		/// as to which version of the application it belonged to.</remarks>
		public string DeserializedInfoApplicationVersion
		{
			get
			{
				object version = this.DeserializeObject(strApplicationVerisonName);
				if(version != null)
					return (string)version;
				else
					return String.Empty;
			}
		}
		/// <summary>
		/// Overloaded. Creates an instance of the AppStateSerializer class.
		/// </summary>
		/// <param name="mode">The <see cref="SerializeMode"/> in which to create.</param>
		/// <param name="persistpath">The persistence path to be used for this mode. See remarks for more info.</param>
		/// <remarks>
		/// <para>The persistpath argument should be based on the <see cref="AppStateSerializer.SerializationMode"/> property, as follows:</para>
		/// <list type="bullet">
		/// <item>SerializeMode.XMLFile<term>A string representing the file name (an .XML suffix will be added) in which to store.</term></item>
		/// <item>SerializeMode.BinaryFile<term>A string representing the file name (a .bin suffix will be added) in which to store.</term></item>
		/// <item>SerializeMode.WindowsRegistry<term>A <see cref="Microsoft.Win32.RegistryKey"/> under which to persist.</term></item>
		/// <item>SerializeMode.IsolatedStorage<term>A string representing the file name (a .bin suffix will be added) in which to store within the Isolated Storage.</term></item>
		/// <item>SerializeMode.ExternalStream<term>An instance of a <see cref="System.IO.Stream"/> derived class with Read, Write, and Seek capability.</term></item>
		/// </list>
		/// <para>
		/// If using the IsolatedStorage mode, then the default isolated storage scope (Domain | Assembly | User) will be used.
		/// Use the three argument constructor to specify a custom scope.
		/// </para>
		/// </remarks>
		public AppStateSerializer(SerializeMode mode, Object persistpath)
			: this(mode, persistpath, AppStateSerializer.issDefault)
		{
		}

		/// <summary>
		/// Creates an instance of the AppStateSerializer class.
		/// </summary>
		/// <param name="mode">The <see cref="SerializeMode"/> in which to create.</param>
		/// <param name="persistpath">The persistence path to be used for this mode. See remarks for more info.</param>
		/// <param name="scope">The <see cref="IsolatedStorageScope"/> to be used if the mode is set to SerializeMode.IsolatedStorage.</param>
		/// <remarks>
		/// <para>The persistpath argument should be based on the <see cref="AppStateSerializer.SerializationMode"/> property, as follows:</para>
		/// <list type="bullet">
		/// <item>SerializeMode.XMLFile<term>A string representing the file name (an .XML suffix will be added) in which to store.</term></item>
		/// <item>SerializeMode.BinaryFile<term>A string representing the file name (a .bin suffix will be added) in which to store.</term></item>
		/// <item>SerializeMode.WindowsRegistry<term>A <see cref="Microsoft.Win32.RegistryKey"/> under which to persist.</term></item>
		/// <item>SerializeMode.IsolatedStorage<term>A string representing the file name (a .bin suffix will be added) in which to store within the Isolated Storage.</term></item>
		/// <item>SerializeMode.ExternalStream<term>An instance of a <see cref="System.IO.Stream"/> derived class with Read, Write, and Seek capability.</term></item>
		/// </list>
		/// <para>
		/// The scope parameter will be referred to if using the IsolatedStorage mode.
		/// </para>
		/// </remarks>
		public AppStateSerializer(SerializeMode mode, Object persistpath, IsolatedStorageScope scope)
		{
			this.serMode = mode;
			if(mode == SerializeMode.WindowsRegistry)
			{
				RegistryKey regkey = persistpath as RegistryKey;
				if(regkey == null)
					throw new ApplicationException("Invalid RegistryKey specified for the PersistencePath.");
				this.serPath = regkey.Name;
			}
			else if((mode == SerializeMode.BinaryFmtStream) || (mode == SerializeMode.XMLFmtStream))
			{
				Stream strm = persistpath as Stream;
				if((strm == null) || (strm.CanRead == false) || (strm.CanSeek == false))
					throw new ApplicationException("Syncfusion AppStateSerializer - Invalid Stream specified for the PersistencePath. The PersistencePath should be a valid Stream instance with Read and Seek capability.");
				this.serPath = persistpath;
			}
			else
				this.serPath = persistpath;			
			this.isoScope = scope;

			this.Init();
		}

		private void Init()
		{
			// If a persisted store is present, then load that information onto the 
			// serialize / deserialize hashtables.
			this.LoadPersistedData();
			if(this.weakRef != null && this.weakRef.IsAlive)
			{
				Hashtable ht = this.weakRef.Target as Hashtable;			
				foreach(String strkey in ht.Keys)
					this.htSerialize.Add(strkey, ht[strkey]);
			}
			// Subscribe to the ApplicationExit event
			//Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
		}	

		/// <summary>
		/// Returns a reference to the unique <see cref="AppStateSerializer"/> instance.
		/// </summary>
		/// <returns>The AppStateSerializer instance.</returns>
		/// <remarks>
		/// This method will be removed in a future version. Please use the GetSingleton method instead.
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Please use the GetSingleton method instead.", false),
		Browsable(false)]
		static public AppStateSerializer GetInstance()
		{
			return AppStateSerializer.GetSingleton();
		}		

		/// <summary>
		/// Returns a reference to the unique <see cref="AppStateSerializer"/> instance.
		/// </summary>
		/// <returns>The AppStateSerializer instance.</returns>
		static public AppStateSerializer GetSingleton()
		{
			if(AppStateSerializer.objInstance == null)
				AppStateSerializer.InitializeSingleton(SerializeMode.IsolatedStorage, AppStateSerializer.strDefault, AppStateSerializer.issDefault);
			return AppStateSerializer.objInstance;
		}

		/// <summary>
		/// Controls the binding of an assembly name to a specific <see cref="System.Reflection.Assembly"/>.
		/// </summary>
		/// <param name="assemblyName">The assembly name string.</param>
		/// <param name="assembly">The corresponding <b>Assembly</b> to bind to.</param>
		/// <remarks>
		/// <para>
		/// The AppStateSerializer class, by default, uses "Simple" assembly names (not strongly typed) to serialize types. This
		/// renders the deserialization process unpredictable because the resultant type of an object after
		/// deserialization is dependent upon the latest version of that assembly installed in the GAC, if any.
		/// This will usually result in casting errors during the deserialization process when the app is linking to an older version of the
		/// assembly and when a newer version of the assembly is installed in the GAC.
		/// </para>
		/// <para>
		/// This method allows you to overcome this limitation by associating a "Simple" assembly name with a
		/// specific <see cref="System.Reflection.Assembly"/>.
		/// </para>
		/// </remarks>
		/// <example>
		/// <para>
		/// This example will bind the
		/// assembly that the app is linking to, to the "Simple" assembly name. Do this from the static
		/// constructor of the class that uses the <b>AppStateSerializer</b>
		/// class:
		/// <code lang="C#">
		/// static MyType()
		/// {
		///         AppStateSerializer.SetBindingInfo("MyNameSpace.MyType", typeof(MyType).Assembly);
		/// }
		/// </code>
		/// <code lang="VB">
		/// 'In type MyType:
		/// Shared  Sub New()
		///     AppStateSerializer.SetBindingInfo("MyNameSpace.MyType", Type.GetType(MyType).Assembly)
		/// End Sub
		/// </code>
		/// </para>
		/// </example>
		public static void SetBindingInfo(string assemblyName, Assembly assembly)
		{
			assemblyName = assemblyName.ToLower();
			if(assembly != null)
				customBinder.AssemblyNamesVsAssembly[assemblyName] = assembly;
			else
				customBinder.AssemblyNamesVsAssembly.Remove(assemblyName);
		}

		/// <summary>
		/// Binds a type in the specified assembly to the same type in a different assembly. Typically useful
		/// in supporting backward compatibility.
		/// </summary>
		/// <param name="assemblyName">The assembly name string.</param>
		/// <param name="typeName">The type name string.</param>
		/// <param name="assembly">The corresponding <b>Assembly</b> to bind to.</param>
		/// <remarks>
		/// <para>
		/// This method is useful when you renamed your assembly and you want to map the old types to the
		/// new types in the new assembly. If you didn't rename the assembly, then just consider using the
		/// <see cref="SetBindingInfo"/> method.
		/// </para>
		/// </remarks>
		public static void SetTypeBindingInfo(string assemblyName, string typeName, Assembly assembly)
		{
			string typePlusAssemblyName = typeName + assemblyName;
			typePlusAssemblyName = typePlusAssemblyName.ToLower();
			if(assembly != null)
				customBinder.TypeNamesVsAssembly[typePlusAssemblyName] = assembly;
			else
				customBinder.TypeNamesVsAssembly.Remove(typePlusAssemblyName);
		}

		/// <summary>
		/// Binds the oldtypename type to the newtypename type. This method comes in handy when serialized types have 
		/// undergone a name change and backward compatibility is to be retained.
		/// </summary>
		/// <param name="oldtypename">The old name of the type.</param>
		/// <param name="newtypename">The new name of the type.</param>
		public static void SetTypeToTypeBindingInfo(string oldtypename, string newtypename)
		{
			if(newtypename != string.Empty)
				customBinder.TypeNameVsTypeName[oldtypename] = newtypename;
			else
				customBinder.TypeNameVsTypeName.Remove(oldtypename);
		}

		/// <summary>
		/// Sets the persistence mode and persistence path for the singleton instance of <see cref="AppStateSerializer"/>.
		/// </summary>
		/// <remarks>The singleton's parameters can only be changed before it gets created. 
		/// It gets created in the first call to the <see cref="AppStateSerializer.GetSingleton"/> method.
		/// You can force the serializer to clear its serialization map by using the 
		/// <see cref="AppStateSerializer.FlushSerializer"/> method.
		/// <para>
		/// This method will be removed in a future version. Please use the InitializeSingleton method instead.
		/// </para>
		/// </remarks>
		/// <param name="mode">A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistpath">An object that represents the persistence medium.</param>
		/// <param name="scope">The IsolatedStorageScope to be used.</param>
		[Obsolete("This method will be removed in a future version. Please use the InitializeSingleton method instead.", false),
		Browsable(false)]
		static public void InitializeSerializer(SerializeMode mode, Object persistpath, IsolatedStorageScope scope)
		{
			AppStateSerializer.InitializeSingleton(mode, persistpath, scope);
		}

		/// <summary>
		/// Overloaded. Sets the persistence mode and persistence path for the singleton instance of <see cref="AppStateSerializer"/>.
		/// </summary>
		/// <remarks>The singleton's parameters can only be changed before it gets created. 
		/// It gets created in the first call to <see cref="AppStateSerializer.GetSingleton"/> method.
		/// You can force the serializer to clear its serialization map by using the 
		/// <see cref="AppStateSerializer.FlushSerializer"/> method.
		/// </remarks>
		/// <param name="mode">A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistpath">An object that represents the persistence medium.</param>
		/// <param name="scope">The IsolatedStorageScope to be used. Referred only when the mode is IsolatedStorage.</param>
		static public void InitializeSingleton(SerializeMode mode, Object persistpath, IsolatedStorageScope scope)
		{
			if(AppStateSerializer.objInstance == null)
			{
				AppStateSerializer.objInstance = new AppStateSerializer(mode, persistpath, scope);
				Application.ApplicationExit += new EventHandler(AppStateSerializer.objInstance.OnApplicationExit);
			}
			else
			{
				Debug.Assert(false, "The Singleton has already been created by a call to GetSingleton.");
				return;
			}
		}		

		/// <summary>
		/// Sets the persistence mode and persistence path for the singleton instance of <see cref="AppStateSerializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>The singleton's parameters can only be changed before it gets created. 
		/// It gets created in the first call to the <see cref="AppStateSerializer.GetSingleton"/> method.
		/// You can force the serializer to clear its serialization map by using the 
		/// <see cref="AppStateSerializer.FlushSerializer"/> method.</para>
		/// <para>
		/// If the mode is set to Isolated Storage, then the default isolated storage scope will be used.
		/// </para>
		/// <para>The persistpath argument should be based on the <see cref="AppStateSerializer.SerializationMode"/> property, as follows:</para>
		/// <list type="bullet">
		/// <item>SerializeMode.XMLFile<term>A string representing the file name (an .XML suffix will be added) in which to store.</term></item>
		/// <item>SerializeMode.BinaryFile<term>A string representing the file name (a .bin suffix will be added) in which to store.</term></item>
		/// <item>SerializeMode.WindowsRegistry<term>A <see cref="Microsoft.Win32.RegistryKey"/> under which to persist.</term></item>
		/// <item>SerializeMode.IsolatedStorage<term>A string representing the file name (a .bin suffix will be added) in which to store within the Isolated Storage.</term></item>
		/// <item>SerializeMode.ExternalStream<term>An instance of a <see cref="System.IO.Stream"/> derived class with Read, Write and Seek capability.</term></item>
		/// </list>
		/// <para/>
		/// </remarks>
		/// <example>
		/// <code lang="C#">
		/// public Form1()
		/// {
		/// 	// To make the singleton use an XML file:
		/// 	AppStateSerializer.InitializeSingleton(SerializeMode.XMLFile, "GlobalState");
		/// 	
		/// 	InitializeComponent();
		/// }
		/// </code>
		/// <code lang="VB">
		/// Public Sub New()
		/// {
		///		' To make the singleton use an XML file:
		///		AppStateSerializer.InitializeSingleton(SerializeMode.XMLFile, "GlobalState")
		///		
		///		InitializeComponent()
		/// }
		/// </code>
		/// </example>
		/// <param name="mode">A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistpath">An object that represents the persistence medium.</param>
		static public void InitializeSingleton(SerializeMode mode, Object persistpath)
		{
			InitializeSingleton(mode, persistpath, AppStateSerializer.issDefault);
		}

		/// <summary>
		/// Clears the serialization map and deletes the persistent store.
		/// <para>
		/// If the persistent store is an external stream, then FlushSerializer just clears the 
		/// serialization map and returns without affecting the stream.
		/// </para>
		/// </summary>
		public void FlushSerializer()
		{
			this.weakRef = null;		
			AppStateSerializer.DeletePersistentStore(this.serMode, this.serPath, 
								this.isoScope, this.htSerialize.Keys);			
			this.htSerialize.Clear();			
		}

		private static void DeletePersistentStore(SerializeMode mode, Object serpath, IsolatedStorageScope scope, ICollection values)
		{
			switch(mode)	
			{
				case SerializeMode.IsolatedStorage:
					IsolatedStorageFile isostore = IsolatedStorageFile.GetStore(scope, null, null);
					string[] filenames = isostore.GetFileNames(ConcatFileName(serpath as String,".bin"));
					foreach(String strfile in filenames)
						isostore.DeleteFile(strfile);
					break;
				case SerializeMode.BinaryFile:
                    String strbinfile = ConcatFileName(serpath as String, ".bin");
					if(File.Exists(strbinfile))
						File.Delete(strbinfile);												
					break;
				case SerializeMode.XMLFile:
                    String strxmlfile = ConcatFileName(serpath as String, ".xml");
					if(File.Exists(strxmlfile))
						File.Delete(strxmlfile);
					break;
				case SerializeMode.WindowsRegistry:
					RegistryKey regkey = null;
					if(serpath.GetType() == typeof(RegistryKey))
						regkey = serpath as RegistryKey;
					else
						regkey = AppStateSerializer.GetRegistryKey(serpath as String);
					if(regkey == null)
						throw new ApplicationException("Invalid RegistryKey specified for the PersistencePath");
					foreach(String strkey in values )
						regkey.DeleteValue(strkey, false);
					regkey.Close();
					break;
			}
		}

		// Invoke SoapFormatter in a separate method to delay loading
		// the *.soap.dll untill it's absolutely necessary.
		private static IFormatter GetSoapFormatter()
		{
			SoapFormatter fmtr = new SoapFormatter();
			fmtr.AssemblyFormat = FormatterAssemblyStyle.Simple;
			fmtr.Binder = customBinder;
			return fmtr;
		}

		private static IFormatter GetFormatter(SerializeMode mode)
		{
			if((mode == SerializeMode.XMLFile) || (mode == SerializeMode.XMLFmtStream))
			{
				return GetSoapFormatter();
			}
			else
			{
				BinaryFormatter fmtr = new BinaryFormatter();
				fmtr.AssemblyFormat = FormatterAssemblyStyle.Simple;
				fmtr.Binder = customBinder;
				return fmtr;
			}
		}

		private static Stream GetStream(SerializeMode mode, Object path, IsolatedStorageScope scope, bool bwrite)
		{
			try
			{
				if (bwrite)
				{
					switch(mode)
					{
						case SerializeMode.IsolatedStorage:
							IsolatedStorageFile isostore = IsolatedStorageFile.GetStore(scope, (Type)null, (Type)null);
							string fileName = ConcatFileName(path as String,".bin");
							string[] files = isostore.GetFileNames(fileName);
							if( files.Length > 0)
								isostore.DeleteFile(fileName);
							return new IsolatedStorageFileStream(fileName, FileMode.CreateNew, isostore);

						case SerializeMode.BinaryFile:
							return File.Open(ConcatFileName(path as String,".bin"), FileMode.Create);
						case SerializeMode.XMLFile:
							return File.Open(ConcatFileName(path as String,".xml"), FileMode.Create);									
						case SerializeMode.WindowsRegistry:
							return new MemoryStream();						
					}
				}
				else
				{
                    switch (mode)
                    {
                        case SerializeMode.IsolatedStorage:
                            IsolatedStorageFile isostore = IsolatedStorageFile.GetStore(scope, (Type)null, (Type)null);
                            string[] filenames = isostore.GetFileNames(ConcatFileName(path as String, ".bin"));
                            foreach (String strfile in filenames)
                                return new IsolatedStorageFileStream(strfile, FileMode.Open, FileAccess.Read, FileShare.Read, isostore);
                            break;
                        case SerializeMode.BinaryFile:
                            String strbinfile = ConcatFileName(path as String, ".bin");
                            if (File.Exists(strbinfile))
                                return File.Open(strbinfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                            break;
                        case SerializeMode.XMLFile:
                            String strxmlfile = ConcatFileName(path as String, ".xml");
                            if (File.Exists(strxmlfile))
                                return File.Open(strxmlfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                            break;
                        case SerializeMode.WindowsRegistry:
                            return new MemoryStream();
                    }
				}
			}
			catch(Exception ex)
			{
                if (AppStateSerializer.shouldThrowException)
                {
                    throw new Exception();
                }
                else
                {
				Debug.Assert(false, "Error - Serialization stream access failed.", ex.Message);
				Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Exception" );
			}
            } 
			return null;							
		}
		
       static private string ConcatFileName(string fileName, string ext)
        {
           if(AppendFileExtension)
               fileName = String.Concat(fileName, ext);
            
           return fileName;
        }
		static private RegistryKey GetRegistryKey(String strregkey)
		{
			String strrootkey = strregkey.Substring(0, strregkey.IndexOf("\\", 0));
			RegistryHive[] allkeys = Enum.GetValues(typeof(RegistryHive)) as RegistryHive[];
			foreach(RegistryHive rh in allkeys)
			{
				RegistryKey rk = RegistryKey.OpenRemoteBaseKey(rh, String.Empty);
				if(rk.Name.Equals(strrootkey))
					return rk.CreateSubKey( strregkey.Substring(strregkey.IndexOf("\\")+1) );
			}
			return null;
		}

		public void RemoveInfoForObject( string strName )
		{
			if( strName == null )
				throw new ArgumentNullException( "strName" );

			if( htSerialize[ strName ] != null )
			{
				htSerialize.Remove( strName );
			}
		}

		/// <summary>
		/// Overloaded. Writes the object to persistent storage under the specified tag.
		/// </summary>
		/// <param name="strname">A descriptor tag for the object.</param>
		/// <param name="obj">The object to be persisted. If NULL, an existing object is removed from the serialization map.</param>
		/// <remarks>
		/// <para>If the <see cref="Enabled"/> is False, then this method will not serialize.</para>
		/// </remarks>
		public void SerializeObject(String strname, Object obj)
		{
			if(!this.Enabled)
				return;

			this.SerializeObject(strname, obj, false);	
		}

		/// <summary>
		/// Writes the object to persistent storage under the specified tag.
		/// </summary>
		/// <param name="strname">A descriptor tag for the object.</param>
		/// <param name="obj">The object to be persisted. Use NULL to remove an existing object from the serialization map.</param>
		/// <param name="infinalize">When this parameter is True, the object is serialized only at 
		/// the point when it is being written to the persistent storage medium. This usually 
		/// happens only when the serializer is being finalized.</param>
		/// <remarks>
		/// <para>If the <see cref="Enabled"/> is False, then this method will not serialize.</para>
		/// </remarks>
		public void SerializeObject(String strname, Object obj, bool infinalize)
		{			
			if(!this.Enabled)
				return;

			if(this.htSerialize.Contains(strname) == true || obj == null)
			{
				this.htSerialize.Remove(strname);
				if(obj == null)
				{
					// If the serialize mode is the registry, then remove the associated value. 
					// For the disk persistence modes, clean-up (file overwrite or removal) will take 
					// place during the final serialization.
					if(this.serMode == SerializeMode.WindowsRegistry)
					{
						RegistryKey regkey = AppStateSerializer.GetRegistryKey(this.serPath as String);
						if(regkey == null)
							throw new ApplicationException("Invalid RegistryKey specified for the PersistencePath.");
						regkey.DeleteValue(strname, false);
						regkey.Close();
					}
					return;
				}
			}

			if(infinalize == true)
			{
				this.htSerialize.Add(strname, obj);
			}
			else	
			{
				// The object may not be valid till Finalization. Serialize the object temporarily 
				// into a memstream and during finalization serialize the memstream value.
				IFormatter fmtr = AppStateSerializer.GetFormatter(this.serMode);
				MemoryStream memstrm = new MemoryStream();
				try
				{
					fmtr.Serialize(memstrm, obj);
				}
				catch(Exception e)
				{
					Debug.Assert(false, "Serialization Failed", e.Message);
					return;
				}

				if((this.serMode == SerializeMode.XMLFile) || (this.serMode == SerializeMode.XMLFmtStream))
				{
					UTF8Encoding encoding = new UTF8Encoding();
					String strxml = encoding.GetString(memstrm.ToArray());
					this.htSerialize.Add(strname, strxml);
				}
				else
				{					
					this.htSerialize.Add(strname, memstrm);
				}
			}
		}

		/// <summary>
		/// Deserializes the object from the persistent store.
		/// </summary>
		/// <param name="strname">The object descriptor.</param>
		/// <returns>The deserialized object.</returns>
		/// <remarks>
		/// <para>If the <see cref="Enabled"/> is False, then this method will not deserialize.</para>
		/// </remarks>
		public Object DeserializeObject(String strname)
		{
			if(!this.Enabled)
				return null;

			// If the object has been recently added / updated to the serialize table, then 
			// get it from there.
			if (this.htSerialize.Contains(strname))
				return this.DeserializeFromHashtable(this.htSerialize, strname);
 
			if (this.weakRef == null || !this.weakRef.IsAlive)
				this.LoadPersistedData();
			if (this.weakRef != null && this.weakRef.IsAlive)
			{
				Hashtable ht = this.weakRef.Target as Hashtable;
				if (ht.Contains(strname))
					return this.DeserializeFromHashtable(ht, strname);
			}
			
			return null;
		}

		private void LoadPersistedData()
		{
			if(!this.Enabled)
				return;

			IFormatter fmtr = AppStateSerializer.GetFormatter(this.serMode);
			Stream strm = null;
			if((this.serMode == SerializeMode.BinaryFmtStream) || (this.serMode == SerializeMode.XMLFmtStream))
				strm = this.serPath as Stream;
			else
				strm = AppStateSerializer.GetStream(this.serMode, this.serPath, this.isoScope, false);
			if(strm == null)
				return;

			Hashtable htdeserialize = null;
			if(this.serMode == SerializeMode.WindowsRegistry)
			{
				htdeserialize = new Hashtable();
				RegistryKey regkey = AppStateSerializer.GetRegistryKey(this.serPath as String);
				if(regkey == null)
					throw new ApplicationException("Invalid RegistryKey specified for the PersistencePath.");
				String[] strvalues = regkey.GetValueNames();
				foreach(String strvalue in strvalues)
				{
					Byte[] arrbytes = regkey.GetValue(strvalue) as Byte[];
					strm.Seek(0, SeekOrigin.Begin);
					strm.Write(arrbytes, 0, arrbytes.Length);
					strm.Seek(0, SeekOrigin.Begin);
					try
					{
						htdeserialize.Add(strvalue, fmtr.Deserialize(strm));					
					}
					catch(Exception /*e*/)
					{
						// Ignore - possibly a redundant value.
					}
				}
				regkey.Close();
			}
			else
			{
				try
				{
					if((strm.Length > 0) && (strm.Position < strm.Length))
						htdeserialize = fmtr.Deserialize(strm) as Hashtable;
				}
				catch(Exception e)
				{
					if( !((this.serMode == SerializeMode.BinaryFmtStream) || (this.serMode == SerializeMode.XMLFmtStream)) )
						strm.Close();
					Debug.Assert(false, "Deserialization Failed", e.Message);
					return;
				}
			}
			if( !((this.serMode == SerializeMode.BinaryFmtStream) || (this.serMode == SerializeMode.XMLFmtStream)) )
				strm.Close();
			this.weakRef = new WeakReference(htdeserialize);
		}

		private Object DeserializeFromHashtable(Hashtable ht, String strname)
		{
			IFormatter fmtr = AppStateSerializer.GetFormatter(this.serMode);
			if(ht.Contains(strname) == true)
			{
				Object obj = ht[strname];
				if(obj.GetType() == typeof(MemoryStream))
				{
					MemoryStream memstream = obj as MemoryStream;
					memstream.Seek(0, SeekOrigin.Begin);
					try
					{
						return fmtr.Deserialize(obj as MemoryStream);
					}
					catch(Exception e)
					{
						Debug.Assert(false, "Deserialization Failed", e.Message);
					}
				}
				else
				{
					if((this.serMode == SerializeMode.XMLFile) || (this.serMode == SerializeMode.XMLFmtStream))
					{
						// The depersisted hashtable will contain a string representation of 
						// the XML data. Convert the contents of the String into a bytearray 
						// and deserialize the bytearray using a memorystream.
						UTF8Encoding encoding = new UTF8Encoding();
						byte[] bytearray = encoding.GetBytes(ht[strname] as String);
						MemoryStream memstream = new MemoryStream(bytearray, 0, bytearray.Length, false);

						Object objret = null;
						try
						{
							objret = fmtr.Deserialize(memstream);
						}
						catch(Exception e)
						{
							Debug.Assert(false, "Deserialization Failed", e.Message);
						}
						finally
						{
							memstream.Close();
						}
						return objret;
					}
					else
					{
						return ht[strname];
					}
				}
			}
			return null;
		}		

		/// <summary>
		/// Overloaded. Serializes the object to the specified persistence medium.
		/// </summary>
		/// <param name="mode">A <see cref="SerializeMode"/> value describing the persistence medium.</param>
		/// <param name="persistpath">Represents the persistence medium.</param>
		/// <param name="strname">A string descriptor for the object.</param>
		/// <param name="obj">The object to be serialized. Use NULL to delete the object's store.</param>
		/// <remarks>
		/// <para>If <see cref="Enabled"/> is False, then this method will not serialize.</para>
		/// <para>
		/// This method has been replaced and will be removed form a future version.
		/// Instead, create a new instance of the AppStateSerializer class (with the mode and persist path)
		/// and then use the <see cref="SerializeObject(string, object, bool)"/> and <see cref="DeserializeObject"/>
		/// methods to persist information. Make sure to call <see cref="PersistNow"/> when done persisting.
		/// </para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Please check the class reference for an alternative.", false),
		Browsable(false)]
		public static void SerializeIsolatedObject(SerializeMode mode, Object persistpath, String strname, Object obj)
		{
			AppStateSerializer.SerializeIsolatedObject(mode, persistpath, 
				AppStateSerializer.issDefault,
				strname, obj);
		}

		/// <summary>
		/// Serializes the object to Isolated Storage.
		/// </summary>
		/// <param name="persistpath">The name of the IsolatedStorageFile.</param>
		/// <param name="scope">The IsolatedStorageScope to be used.</param>
		/// <param name="strname">A string descriptor for the object.</param>
		/// <param name="obj">The object to be serialized. Use NULL to delete the object's store.</param>
		/// <remarks>
		/// <para>If <see cref="Enabled"/> is False, then this method will not serialize.</para>
 		/// <para>
		/// This method has been replaced and will be removed form a future version.
		/// Instead, create a new instance of the AppStateSerializer class (with the mode and persist path)
        /// and then use the <see cref="SerializeObject(string, object, bool)"/> and <see cref="DeserializeObject"/>
		/// methods to persist information. Make sure to call <see cref="PersistNow"/> when done persisting.
		/// </para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Please check the class reference for an alternative.", false),
		Browsable(false)]
		public static void SerializeIsolatedObject(Object persistpath, IsolatedStorageScope scope, String strname, Object obj)
		{
			AppStateSerializer.SerializeIsolatedObject(SerializeMode.IsolatedStorage, persistpath, 
																			scope, strname, obj);
		}

		private static void SerializeIsolatedObject(SerializeMode mode, Object persistpath, IsolatedStorageScope scope, String strname, Object obj)
		{
			if(obj != null)
			{
				IFormatter fmtr = AppStateSerializer.GetFormatter(mode);				
				Stream strm = null;
				if((mode == SerializeMode.BinaryFmtStream) || (mode == SerializeMode.XMLFmtStream))
				{
					strm = persistpath as Stream;
					if((strm == null) || (strm.CanRead == false) || (strm.CanWrite == false) || (strm.CanSeek == false))
						throw new ApplicationException("Syncfusion AppStateSerializer - Invalid Stream specified for the PersistencePath. The PersistencePath should be a valid Stream instance with Read, Write, and Seek capability.");
				}
				else
					strm = AppStateSerializer.GetStream(mode, persistpath, scope, true);
				if(strm == null)
					return;

				try
				{
					if(mode == SerializeMode.WindowsRegistry)
					{
						RegistryKey regkey = persistpath as RegistryKey;
						if(regkey == null)
							throw new ApplicationException("Invalid RegistryKey specified for the PersistencePath.");
						fmtr.Serialize(strm, obj);					
						regkey.SetValue(strname, (strm as MemoryStream).ToArray());
						regkey.Close();
					}
					else
					{
						fmtr.Serialize(strm, obj);											
					}
				}
				catch(Exception e)
				{
					Debug.Assert(false, "Serialization Failed", e.Message);
				}
				finally
				{
					if( !((mode == SerializeMode.BinaryFmtStream) || (mode == SerializeMode.XMLFmtStream)) )
						strm.Close();
				}
			}
			else
			{
				AppStateSerializer.DeletePersistentStore(mode, persistpath, scope, new String[]{strname} );
			}
		}

		/// <summary>
		/// Overloaded. Deserializes an object from the specified persistent store.
		/// </summary>
		/// <param name="mode">A <see cref="SerializeMode"/> value describing the persistence medium.</param>
		/// <param name="persistpath">Represents the persistence medium.</param>
		/// <param name="strname">A string descriptor for the object.</param>
		/// <returns>The deserialized object.</returns>
		/// <remarks>
		/// <para>If <see cref="Enabled"/> is False, then this method will not deserialize.</para>
		/// <para>
		/// This method has been replaced and will be removed form a future version.
		/// Instead, create a new instance of the AppStateSerializer class (with the mode and persist path)
        /// and then use the <see cref="SerializeObject(string, object, bool)"/> and <see cref="DeserializeObject"/>.
		/// </para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Please check the class reference for an alternative.", false),
		Browsable(false)]
		public static Object DeserializeIsolatedObject(SerializeMode mode, Object persistpath, String strname)
		{
			return AppStateSerializer.DeserializeIsolatedObject(mode, persistpath, 
				AppStateSerializer.issDefault,
				strname);
		}

		/// <summary>
		/// Deserializes an object from Isolated Storage.
		/// </summary>
		/// <param name="persistpath">The name of the IsolatedStorageFile.</param>
		/// <param name="scope">The IsolatedStorageScope to be used.</param>
		/// <param name="strname">A string descriptor for the object.</param>
		/// <returns>The deserialized object.</returns>
		/// <remarks>
		/// <para>If <see cref="Enabled"/> is False, then this method will not deserialize.</para>
		/// <para>
		/// This method has been replaced and will be removed form a future version.
		/// Instead, create a new instance of the AppStateSerializer class (with the mode and persist path)
        /// and then use the <see cref="SerializeObject(string, object, bool)"/> and <see cref="DeserializeObject"/>.
		/// </para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Please check the class reference for an alternative.", false),
		Browsable(false)]
		public static Object DeserializeIsolatedObject(Object persistpath, IsolatedStorageScope scope, String strname)
		{
			return AppStateSerializer.DeserializeIsolatedObject(SerializeMode.IsolatedStorage, 
																persistpath, scope, strname);
		}

		private static Object DeserializeIsolatedObject(SerializeMode mode, Object persistpath, IsolatedStorageScope scope, String strname)
		{
			Object objreturn = null;
			IFormatter fmtr = AppStateSerializer.GetFormatter(mode);
			Stream strm = null;
			if((mode == SerializeMode.BinaryFmtStream) || (mode == SerializeMode.XMLFmtStream))
			{
				strm = persistpath as Stream;
				if((strm == null) || (strm.CanRead == false) || (strm.CanSeek == false))
					throw new ApplicationException("Syncfusion AppStateSerializer - Invalid Stream specified for the PersistencePath. The PersistencePath should be a valid Stream instance with Read and Seek capability.");
			}
			else
				strm = AppStateSerializer.GetStream(mode, persistpath, scope, false);
			if(strm == null)
				return null;

			try
			{
				if(mode == SerializeMode.WindowsRegistry)
				{
					RegistryKey regkey = persistpath as RegistryKey;
					if(regkey == null)
						throw new ApplicationException("Invalid RegistryKey specified for the PersistencePath");
					byte[] bytearray = regkey.GetValue(strname) as byte[];
					regkey.Close();
					if(bytearray != null)
					{
						strm.Seek(0, SeekOrigin.Begin);
						strm.Write(bytearray, 0, bytearray.Length);
						strm.Seek(0, SeekOrigin.Begin);					
						objreturn = fmtr.Deserialize(strm);	
					}
				}
				else
				{
					objreturn = fmtr.Deserialize(strm);					
				}
			}
			catch(Exception e)
			{
				Debug.Assert(false, "Deserialization Failed", e.Message);
			}
			finally
			{
				if( !((mode == SerializeMode.BinaryFmtStream) || (mode == SerializeMode.XMLFmtStream)) )
					strm.Close();
			}
			return objreturn;
		}

		/// <summary>
		/// Occurs just before the contents of the <see cref="AppStateSerializer"/> are persisted. 
		/// </summary>
		public event EventHandler BeforePersist;

		private void OnBeforePersist(EventArgs e)
		{
			// Saves the application version of the current application.
			this.SerializeObject(strApplicationVerisonName,Application.ProductVersion);

			if(this.BeforePersist != null)
			{
				this.BeforePersist(this, e);
			}
		}

		/// <summary>
		/// Writes the <see cref="AppStateSerializer"/>'s contents to the persistent storage.
		/// </summary>
		/// <remarks>
		/// <para>If <see cref="Enabled"/> is False, then this method will not persist.</para>
		/// </remarks>
		public void PersistNow()
		{
			if(!this.Enabled)
				return;

			this.OnBeforePersist(EventArgs.Empty);
			
			if(this.htSerialize.Count > 0)
			{
				IFormatter fmtr = AppStateSerializer.GetFormatter(this.serMode);
				Stream strm = null;
				if((this.serMode == SerializeMode.BinaryFmtStream) || (this.serMode == SerializeMode.XMLFmtStream))
				{
					strm = this.serPath as Stream;
					if(strm.CanWrite == false)
						throw new ApplicationException("Syncfusion AppStateSerializer - Invalid Stream specified for the PersistencePath. The PersistencePath should be a valid Stream instance with Read, Write, and Seek capability.");
				}
				else
					strm = AppStateSerializer.GetStream(this.serMode, this.serPath, this.isoScope, true);
				if(strm == null)
					return;

				try
				{
					if(this.serMode == SerializeMode.WindowsRegistry)
					{
						RegistryKey regkey = AppStateSerializer.GetRegistryKey(this.serPath as String);
						if(regkey == null)
							throw new ApplicationException("Invalid RegistryKey specified for the PersistencePath");

						foreach(String name in this.htSerialize.Keys)
						{
							Object obj = this.htSerialize[name];
							if(obj.GetType() == typeof(MemoryStream))
								strm = obj as MemoryStream;
							else
								fmtr.Serialize(strm, obj);						
							regkey.SetValue(name, (strm as MemoryStream).ToArray());
							strm.Position = 0;
						}
						regkey.Close();
					}
					else
					{
						fmtr.Serialize(strm, this.htSerialize);
					}
				}
				catch(Exception e)
				{
					Debug.Assert(false, "Serialization Failed", e.Message);
				}
				finally
				{
					if( !((this.serMode == SerializeMode.BinaryFmtStream) || (this.serMode == SerializeMode.XMLFmtStream)) )
						strm.Close();
					this.weakRef = null;
					this.htSerialize.Clear();
				}
			}
			else	// Wipe out any persistent stores that may be present.
			{
				this.FlushSerializer();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public void OnApplicationExit(Object sender, EventArgs e)
		{
			if(!this.Enabled)
				return;

			this.PersistNow();
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CustomSerializationBinder : SerializationBinder
	{
		Hashtable bindingInfo = new Hashtable();
		Hashtable typeNameVsAssembly = new Hashtable();
		Hashtable typeNameVsTypeName = new Hashtable();

		public CustomSerializationBinder()
		{
			// Sometimes, framework has trouble finding the following framework dlls while serializing.
			Type type = typeof(System.Drawing.Point);
			this.bindingInfo[type.Assembly.GetName().Name.ToLower()] = type.Assembly;
			type = typeof(System.Windows.Forms.Form);
			this.bindingInfo[type.Assembly.GetName().Name.ToLower()] = type.Assembly;
			type = typeof(System.ComponentModel.Component);
			this.bindingInfo[type.Assembly.GetName().Name.ToLower()] = type.Assembly;
		}

		public Hashtable AssemblyNamesVsAssembly
		{
			get{return this.bindingInfo;}
		}

		public Hashtable TypeNamesVsAssembly
		{
			get{return this.typeNameVsAssembly;}
		}

		public Hashtable TypeNameVsTypeName
		{
			get { return this.typeNameVsTypeName; }
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			if(this.typeNameVsTypeName[typeName] != null)
			{
				typeName = this.typeNameVsTypeName[typeName] as String;
			}

			string typePlusAssemblyName = typeName + assemblyName;
			typePlusAssemblyName = typePlusAssemblyName.ToLower();
			if(this.typeNameVsAssembly[typePlusAssemblyName] != null)
			{
				Assembly assembly = this.typeNameVsAssembly[typePlusAssemblyName] as Assembly;
				return assembly.GetType(typeName);
			}

			assemblyName = assemblyName.ToLower();
			if(this.bindingInfo[assemblyName] != null)
			{
				Assembly assembly = this.bindingInfo[assemblyName] as Assembly;
				return assembly.GetType(typeName);
			}

			int nseparator = assemblyName.IndexOf(",");
			if(nseparator > 0)
			{
				string shortname = assemblyName.Substring(0, nseparator);
				if(this.bindingInfo[shortname] != null)
				{
					Assembly assembly = this.bindingInfo[shortname] as Assembly;
					return assembly.GetType(typeName);
				}
			}
			
			return null;
		}
	}
	
}
