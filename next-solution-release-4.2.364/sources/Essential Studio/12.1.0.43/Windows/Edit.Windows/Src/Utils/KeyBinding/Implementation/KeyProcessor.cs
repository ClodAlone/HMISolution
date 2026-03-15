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
using System.Xml;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using Syncfusion.XmlSerializersCreator;
using Syncfusion.Shared.Utils.KeyBinding;
using Syncfusion.Windows.Forms.Edit;

namespace Syncfusion.Shared.Utils.KeyBinding.Implementation
{
	/// <summary>
	/// Main key processor.
	/// </summary>
	[Serializable]
	[TypeConverter( typeof( KeyProcessorConverter ) )]
	public class KeyProcessor
		: IEditableObject
		, ISerializable
	{
		#region Internal Classes

		#region KeyCommandBinderImpl
		/// <summary>
		/// Implementation of the IKeyCommandBinder interface.
		/// Used to keep information about single key=command binding.
		/// </summary>
		[XmlRoot( "Binding" )]
		public class KeyCommandBinderImpl
		: IKeyCommandBinder
		{
			#region Class members
			/// <summary>
			/// Parent list, binding belongs to.
			/// </summary>
			private IKeyCommandListBinder m_Parent;
			/// <summary>
			/// Command, that is binded. 
			/// </summary>
			private IKeyCommand m_Command;
			/// <summary>
			/// Key, that is binded. 
			/// </summary>
			protected Keys m_Key;
			/// <summary>
			/// Flag that specifies whether binding was linked to command.
			/// </summary>
			private bool m_initialized;
			/// <summary>
			/// Flag that specifies whether binding was linked to key.
			/// </summary>
			private bool m_initializedKey;
			/// <summary>
			/// KeyProcessor, the list belongs to.
			/// </summary>
			private KeyProcessor m_Processor;
			/// <summary>
			/// Name of the command to be linked with.
			/// </summary>
			private string m_CommandName;
			/// <summary>
			/// Keys converter.
			/// </summary>
			private static KeysConverter m_converter = new KeysConverter();
			#endregion

			#region Class Properties
			/// <summary>
			/// Gets command, that is binded. 
			/// </summary>
			[XmlIgnore]
			public IKeyCommand Command
			{
				get
				{
					if( !m_initialized )
					{
						if( m_Processor == null )
							throw new NullReferenceException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_132 );

						if( m_CommandName != string.Empty && m_CommandName != null )
							m_Command = m_Processor.Commands[ m_CommandName ];

						m_initialized = true;
					}

					return m_Command;
				}
			}
			/// <summary>
			/// Gets or sets command name, just for XML Serialization support.
			/// </summary>
			[XmlAttribute( "Command" )]
			[DefaultValue( "" )]
			public string CommandName
			{
				get
				{
					return m_CommandName;
				}
				set
				{
					m_CommandName = value;
					m_initialized = false;
				}
			}
			/// <summary>
			/// Gets key, that is binded. 
			/// </summary>
			[XmlIgnore]
			public Keys Key
			{
				get
				{
					return m_Key;
				}
			}
			/// <summary>
			/// Gets or sets key value. Intended for XML Serialization only.
			/// </summary>
			[XmlAttribute( "Key" )]
			[DefaultValue( typeof( string ), "None" )]
			public string KeyXML
			{
				get
				{
					return m_converter.ConvertToInvariantString( m_Key );
				}
				set
				{
					if( m_initializedKey )
						throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_133 );

					m_Key = ( Keys )m_converter.ConvertFromInvariantString( value );

					m_initializedKey = true;
				}
			}
			/// <summary>
			/// Gets or sets key processor.
			/// </summary>
			[XmlIgnore]
			public virtual KeyProcessor Processor
			{
				get
				{
					return m_Processor;
				}
				set
				{
					if( value == null )
						throw new ArgumentNullException( "Processor", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_134 );

					m_Processor = value;
				}
			}
			/// <summary>
			/// Gets or sets parent list.
			/// </summary>
			[XmlIgnore]
			public IKeyCommandListBinder Parent
			{
				get
				{
					return m_Parent;
				}
				set
				{
					if( value == null )
						throw new ArgumentNullException( "Parent", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_135 );

					m_Parent = value;
				}
			}
			#endregion

			#region Class Public Methods
			/// <summary>
			/// Tries to process key.
			/// </summary>
			/// <param name="key">Key to be processed.</param>
			/// <returns>True if key was processed, otherwise false.</returns>
			public virtual bool ProcessKey( Keys key )
			{
				if( key == m_Key )
				{
					if( m_Command != null )
						m_Command.Execute();

					return true;
				}

				return false;
			}
			/// <summary>
			/// Gets full name of the combination, current binding is related to.
			/// </summary>
			/// <returns>String that represents currently used combination.</returns>
			public string GetCombinationName()
			{
				if( m_Key == Keys.None )
					return string.Empty;

				string result = m_converter.ConvertToInvariantString( m_Key );

				if( m_Parent != null )
				{
					string parent_str = m_Parent.GetCombinationName();

					if( parent_str != string.Empty )
						result = parent_str + ", " + result;
				}

				return result;
			}

			/// <summary>
			/// Return combination name.
			/// </summary>
			/// <returns>String, that represents current key sequence.</returns>
			public override string ToString()
			{
				return GetCombinationName();
			}

			/// <summary>
			/// Reset links to commands, and leaves only by-name references.
			/// </summary>
			public virtual void ResetCommandLinks()
			{
				m_initialized = false;
			}
			#endregion

			#region Class Initialize/Finalize methods
			/// <summary>
			/// Constructor for XML serialization.
			/// </summary>
			public KeyCommandBinderImpl()
			{
			}
			/// <summary>
			/// Creates new instance of the class and initializes it.
			/// </summary>
			/// <param name="processor">Key process, the binding belongs to.</param>
			/// <param name="parent">Parent list of the binding.</param>
			/// <param name="command">Command, the binding is linked to.</param>
			/// <param name="key">Key, the binding is linked to.</param>
			public KeyCommandBinderImpl( KeyProcessor processor, IKeyCommandListBinder parent, IKeyCommand command, Keys key )
			{
				if( processor == null )
					throw new ArgumentNullException( "processor" );

				m_Parent = parent;
				m_Key = key;
				m_Command = command;

				if( m_Command != null )
					m_CommandName = m_Command.Name;

				m_Processor = processor;
				m_initialized = true;
			}
			#endregion
		}
		#endregion

		#region KeyCommandListBinderImpl
		/// <summary>
		/// List of key bindings.
		/// </summary>
		[XmlRoot( "BindingList" )]
		public class KeyCommandListBinderImpl
		: KeyCommandBinderImpl
		, IKeyCommandListBinder
		{
			#region Class members
			/// <summary>
			/// Hashtable with keybindings.
			/// Key - key, Value - IKeyCommandBinder/IKeyCommandListBinder
			/// </summary>
			private Hashtable m_Bindings = new Hashtable();
			#endregion

			#region Class Initialize/Finalize methods
			/// <summary>
			/// For XML Serialization only.
			/// </summary>
			public KeyCommandListBinderImpl()
			{
			}
			/// <summary>
			/// Creates new instance of the class and initializes it.
			/// </summary>
			/// <param name="processor">Main keys processor.</param>
			/// <param name="parent">Parent of the list.</param>
			/// <param name="key">Key, the binding is linked to.</param>
			public KeyCommandListBinderImpl( KeyProcessor processor, IKeyCommandListBinder parent, Keys key )
				: base( processor, parent, null, key )
			{
				if( processor == null )
					throw new ArgumentNullException( "processor" );
			}
			#endregion

			#region Class Public Methods
			/// <summary>
			/// Sets binding of the key to specified command.
			/// </summary>
			/// <param name="key">Key to be binded.</param>
			/// <param name="command">Name of the command, the key is to be binded to.</param>
			/// <returns>Command if binding 
			/// compleated successfully, or null of binding failed.</returns>
			/// <remarks>
			/// It is not necessary to create command before binding. If it does not exists, it will be created.
			/// </remarks>
			public IKeyCommand BindToCommand( Keys key, string command )
			{
				IKeyCommand commandInstance = Processor.Commands[ command ];

				if( commandInstance == null )
					commandInstance = Processor.Commands.Add( command );

				KeyCommandBinderImpl binding = new KeyCommandBinderImpl( Processor, this, commandInstance, key );
				m_Bindings[ key ] = binding;

				return commandInstance;
			}
			/// <summary>
			/// Sets binding for the key to the new command.
			/// </summary>
			/// <param name="key">Key to be binded.</param>
			/// <returns>Returns existing binder, or creates new if key was not binded before or was binded to command.</returns>
			public IKeyCommandListBinder BindToBinder( Keys key )
			{
				IKeyCommandListBinder bindedList = m_Bindings[ key ] as IKeyCommandListBinder;

				if( bindedList == null )
				{
					bindedList = new KeyCommandListBinderImpl( Processor, this, key );
					m_Bindings[ key ] = bindedList;
				}

				return bindedList;
			}
			/// <summary>
			/// Removes any associated binding for the specified key.
			/// </summary>
			/// <param name="key">Key to be unbinded.</param>
			public void RemoveBinding( Keys key )
			{
				if( m_Bindings.Contains( key ) )
					m_Bindings.Remove( key );
			}
			/// <summary>
			/// Searches for bindings of the command.
			/// </summary>
			/// <param name="command">Name of the command.</param>
			/// <returns>Bindings, that are assigned to that command.</returns>
			public IKeyCommandBinder[] FindBindings( string command )
			{
				ArrayList result = new ArrayList();

				foreach( IKeyCommandBinder binder in m_Bindings.Values )
				{
					IKeyCommandListBinder list = binder as IKeyCommandListBinder;

					if( list != null )
					{
						IKeyCommandBinder[] foundList = list.FindBindings( command );

						if( foundList != null )
							result.AddRange( foundList );
					}
					else
					{
						if( binder.Command != null &&
							binder.Command.Name == command )
							result.Add( binder );
					}
				}

				return ( IKeyCommandBinder[] )result.ToArray( typeof( IKeyCommandBinder ) );
			}
			/// <summary>
			/// Searches for binding of the keys sequence.
			/// </summary>
			/// <param name="keySequence">Key sequence to find.</param>
			/// <param name="iStart">Index in keySequence to start with.</param>
			/// <returns>Bindings, that are assigned to that command.</returns>
			public IKeyCommandBinder FindBinding( Keys[] keySequence, int iStart )
			{
				if( keySequence == null )
					throw new ArgumentNullException( "keySequence" );

				if( iStart < 0 || iStart > keySequence.Length - 1 ) throw new ArgumentOutOfRangeException(
					"iStart", iStart, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_136 );

				IKeyCommandBinder result = this[ keySequence[ iStart ] ];

				if( result == null )
					return null;

				IKeyCommandListBinder list = result as IKeyCommandListBinder;

				if( list != null )
				{
					if( iStart < keySequence.Length - 1 )
					{
						return list.FindBinding( keySequence, iStart + 1 );
					}
					else return null;
				}
				else return result;
			}
			/// <summary>
			/// Tries to process key.
			/// </summary>
			/// <param name="key">Key to be processed.</param>
			/// <returns>True if key was processed.</returns>
			public override bool ProcessKey( Keys key )
			{
				IKeyCommandBinder result = this[ key ];

				if( result != null && result.Command != null )
				{
					result.Command.Execute();
					return true;
				}

				return false;
			}

			/// <summary>
			/// Reset links to commands, and leaves only by-name references.
			/// </summary>
			public override void ResetCommandLinks()
			{
				base.ResetCommandLinks();

				foreach( KeyCommandBinderImpl binding in m_Bindings.Values )
				{
					binding.ResetCommandLinks();
				}
			}
			#endregion

			#region Class Properties
			/// <summary>
			/// Gets or sets key processor.
			/// </summary>
			[XmlIgnore]
			public override KeyProcessor Processor
			{
				get
				{
					return base.Processor;
				}
				set
				{
					base.Processor = value;

					foreach( KeyCommandBinderImpl binder in m_Bindings.Values )
					{
						binder.Processor = value;
					}

				}
			}
			/// <summary>
			/// Gets binding for the key.
			/// </summary>
			[XmlIgnore]
			public IKeyCommandBinder this[ Keys key ]
			{
				get
				{
					return m_Bindings[ key ] as IKeyCommandBinder;
				}
			}
			/// <summary>
			/// Gets or sets array of bindings. Intended just for XML Serialization.
			/// </summary>
			[XmlArray( "Bindings", IsNullable = true )]
			[XmlArrayItem( "Binding", typeof( KeyCommandBinderImpl ) )]
			[XmlArrayItem( "BindingList", typeof( KeyCommandListBinderImpl ) )]
			public KeyCommandBinderImpl[] SubBundings
			{
				get
				{
					KeyCommandBinderImpl[] result = new KeyCommandBinderImpl[ m_Bindings.Count ];
					m_Bindings.Values.CopyTo( result, 0 );

					return result;
				}
				set
				{
					m_Bindings.Clear();

					if( value == null )
						return;

					for( int i = 0; i < value.Length; i++ )
					{
						m_Bindings.Add( value[ i ].Key, value[ i ] );
						value[ i ].Parent = this;
					}
				}
			}
			#endregion
		}
		#endregion

		#endregion

		#region Class Constans
		/// <summary>
		/// Name of the value for serialization.
		/// </summary>
		private const string DEF_XML_DATA = "XmlData";
		#endregion

		#region Class members
		/// <summary>
		/// Root key binder.
		/// </summary>
		private KeyCommandListBinderImpl m_RootBinder;
		/// <summary>
		/// Current key binder.
		/// </summary>
		private IKeyCommandListBinder m_CurrentBinder;
		/// <summary>
		/// List of commands.
		/// </summary>
		private KeyCommandListImpl m_Commands = new KeyCommandListImpl();
		/// <summary>
		/// Keys converter.
		/// </summary>
		private KeysConverter m_Converter;
		/// <summary>
		/// Xml serializer for serialization/deserialization of bindings.
		/// </summary>
		private XmlSerializer m_serializer;
		/// <summary>
		/// Stream, that keeps backup version of the key-bindings.
		/// </summary>
		private MemoryStream m_backupStream;
		#endregion

		#region Class Events
		/// <summary>
		/// Event, that is raised when some key was unprocesses.
		/// </summary>
		public event ProcessCommandsEventHandler UnprocessedKey;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets list of commands.
		/// </summary>
		public IKeyCommandList Commands
		{
			get
			{
				return m_Commands;
			}
		}
		/// <summary>
		/// Gets root key binder.
		/// </summary>
		public IKeyCommandListBinder Binder
		{
			get
			{
				return m_RootBinder;
			}
		}
		/// <summary>
		/// Gets key converter.
		/// </summary>
		public KeysConverter Converter
		{
			get
			{
				if( m_Converter == null )
					m_Converter = new KeysConverter();

				return m_Converter;
			}
		}
		/// <summary>
		/// Gets Xml serializer for bindings list.
		/// </summary>
		private XmlSerializer Serializer
		{
			get
			{
				if( m_serializer == null )
					try
					{
						m_serializer = SerializersManager.GetSerializer( typeof( KeyCommandListBinderImpl ) );
					}
					catch( Exception ex )
					{
						// HACK: Pull our assembly base file name from exception message
						Regex regex = new Regex( @"File or assembly name (?<baseFileName>.*).dll" );
						Match match = regex.Match( ex.Message );
						string baseFileName = match.Groups[ "baseFileName" ].Value;

						string outputPath = Path.Combine( Path.GetTempPath(), baseFileName + ".out" );
						Debug.WriteLine( ( new StreamReader( outputPath ) ).ReadToEnd() );
						Debug.WriteLine( "" );

						string csPath = Path.Combine( Path.GetTempPath(), baseFileName + ".0.cs" );
						Debug.WriteLine( "XmlSerializer-produced source:\n" + csPath );

						Debug.WriteLine( ex.Message );
					}

				return m_serializer;
			}
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Process key.
		/// </summary>
		/// <param name="key">Key to be processed.</param>
		public void ProcessKey( Keys key )
		{
			bool disallowed = false;

			Keys checkKey = key & Keys.KeyCode;
			disallowed |= ( checkKey == Keys.ControlKey );
			disallowed |= ( checkKey == Keys.ShiftKey );
			disallowed |= ( checkKey == Keys.Menu );
			disallowed |= ( checkKey == Keys.Capital );
			disallowed |= ( checkKey == Keys.Scroll );
			disallowed |= ( checkKey == Keys.NumLock );

			if( ( !m_CurrentBinder.ProcessKey( key ) ) & !disallowed )
			{
				IKeyCommandListBinder list = m_CurrentBinder[ key ] as IKeyCommandListBinder;

				m_CurrentBinder = ( list == null ) ? m_RootBinder : list;

				if( list == null )
					OnUnprocessedKey( key );
			}
			else
			{
				if( !disallowed )
				{
					m_CurrentBinder = m_RootBinder;
				}
			}
		}
		/// <summary>
		/// Saves bindings to XML.
		/// </summary>
		/// <param name="stream">Output stream.</param>
		public void SaveBindingsToXML( Stream stream )
		{
			if( stream == null )
				throw new ArgumentNullException( "stream" );

			Serializer.Serialize( stream, m_RootBinder );
		}
		/// <summary>
		/// Loads bindings from XML.
		/// </summary>
		/// <param name="stream">Input stream.</param>
		public void LoadBindingsFromXML( Stream stream )
		{
			if( stream == null )
				throw new ArgumentNullException( "stream" );

			m_RootBinder = ( KeyCommandListBinderImpl )Serializer.Deserialize( stream );
			m_RootBinder.Processor = this;
			m_CurrentBinder = m_RootBinder;
		}
		/// <summary>
		/// Serializes list.
		/// </summary>
		/// <param name="info">Serialization info.</param>
		/// <param name="context">Serialization context.</param>
		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			MemoryStream stream = new MemoryStream();

			try
			{
				SaveBindingsToXML( stream );
			}
			catch( Exception ex )
			{
				MessageBox.Show( "Can not serialize key bindings. Exception message: \n" + ex.Message, "Key bindings" );
			}

			if( stream.Length > 0 )
				info.AddValue( DEF_XML_DATA, stream.ToArray(), typeof( byte[] ) );

			stream.Close();
		}

		/// <summary>
		/// Scans for commands and key-bindings in custom attributes of all public methods of the class.
		/// </summary>
		/// <param name="instance">Instance to be scanned.</param>
		public void InitializeClassDefaults( IKeyBinderContainer instance )
		{
			if( instance == null )
				throw new ArgumentNullException( "instance" );

			Commands.Clear();
			m_CurrentBinder = m_RootBinder = new KeyCommandListBinderImpl( this, null, Keys.None );

			AppendKeyBindings( instance, true, true );
		}
		/// <summary>
		/// Initializes list of the commands.
		/// </summary>
		/// <param name="instance"></param>
		public void InitializeCommandsList( IKeyBinderContainer instance )
		{
			if( instance == null )
				throw new ArgumentNullException( "instance" );

			Commands.Clear();
			m_RootBinder.ResetCommandLinks();

			AppendKeyBindings( instance, true, false );
		}
		/// <summary>
		/// Adds commands and keybindings. 
		/// </summary>
		/// <param name="instance">Instance to analyse.</param>
		/// <param name="addCommands">Indicates whether commands should be added.</param>
		/// <param name="addKeys">Indicates whether keys should be added.</param>
		public void AppendKeyBindings( object instance, bool addCommands, bool addKeys )
		{
			if( null == instance )
				throw new ArgumentNullException( "instance" );

			Type type = instance.GetType();
			IKeyBinderContainer container = instance as IKeyBinderContainer;

			MethodInfo[] methods = type.GetMethods( BindingFlags.Instance | BindingFlags.Public );

			for( int i = 0, len = methods.Length; i < len; i++ )
			{
				MethodInfo method = methods[ i ];

				CommandAttribute[] attribs = ( CommandAttribute[] )method.GetCustomAttributes(
					typeof( CommandAttribute ), true );

				if( attribs == null || attribs.Length == 0 ) continue;

				// Command attribute can be just one.
				CommandAttribute attribute = attribs[ 0 ] as CommandAttribute;

				IKeyCommand command = Commands[ attribute.Name ] as IKeyCommand;

				if( addCommands )
				{
					ProcessCommandEventHandler handler = ( ProcessCommandEventHandler )
						Delegate.CreateDelegate( typeof( ProcessCommandEventHandler ),
						instance, method.Name );

					if( command == null )
						Commands.Add( attribute.Name ).ProcessCommand += handler;
					else
						command.ProcessCommand += handler;
				}

				if( addKeys )
				{
					// Multiplie key assignment means embebbed key assignments.
					KeysBindingAttribute[] keys = ( KeysBindingAttribute[] )method.GetCustomAttributes(
						typeof( KeysBindingAttribute ), true );

					for( int k = 0, len3 = keys.Length; k < len3; k++ )
					{
						IKeyCommandListBinder binder = Binder;

						for( int n = 0, len4 = keys[ k ].Keys.Length; n < len4; n++ )
						{
							if( n + 1 < len4 )
								binder = binder.BindToBinder( keys[ k ].Keys[ n ] );
							else
								binder.BindToCommand( keys[ k ].Keys[ n ], attribute.Name );
						}
					}
				}
			}

			if( null != container )
			{
				if( addCommands )
					container.RegisterKeyCommands();

				if( addKeys )
					container.RegisterDefaultKeyBindings();
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Deserializes key bindings.
		/// </summary>
		/// <param name="info">Serialization info.</param>
		/// <param name="context">Streaming context.</param>
		private KeyProcessor( SerializationInfo info, StreamingContext context )
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();

			while( enumerator.MoveNext() )
			{
				if( enumerator.Name == DEF_XML_DATA )
				{
					byte[] streamData = ( byte[] )enumerator.Value;
					MemoryStream stream = new MemoryStream( streamData );
					LoadBindingsFromXML( stream );
					break;
				}
			}
		}
		/// <summary>
		/// Creates new key processor and creates new command list 
		/// and bindings list within it.
		/// </summary>
		public KeyProcessor()
		{
			DefaultInitialization();
		}
		/// <summary>
		/// Creates new key processor
		/// </summary>
		/// <param name="stream">Stream to read bindings from.</param>
		public KeyProcessor( Stream stream )
		{
			if( stream == null )
				throw new ArgumentNullException( "stream" );

			LoadBindingsFromXML( stream );
		}
		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Performs default initialization.
		/// </summary>
		protected void DefaultInitialization()
		{
			m_RootBinder = new KeyCommandListBinderImpl( this, null, Keys.None );
			m_CurrentBinder = m_RootBinder;
		}
		#endregion

		#region Class Virtuals
		/// <summary>
		/// Raises UnprocessedKey event.
		/// </summary>
		/// <param name="key">Key, that was not processed.</param>
		protected virtual void OnUnprocessedKey( Keys key )
		{
			if( UnprocessedKey != null )
			{
				UnprocessedKey( key );
			}
		}
		#endregion

		#region IEditableObject implementation
		/// <summary>
		/// Starts editing.
		/// </summary>
		public void BeginEdit()
		{
			m_backupStream = new MemoryStream();

			try
			{
				SaveBindingsToXML( m_backupStream );
				m_backupStream.Position = 0;
			}
			catch
			{
				m_backupStream = null;
				throw;
			}

		}
		/// <summary>
		/// Cancels all changes.
		/// </summary>
		public void CancelEdit()
		{
			if( m_backupStream != null )
			{
				LoadBindingsFromXML( m_backupStream );

				m_backupStream.Close();
				m_backupStream = null;
			}
		}
		/// <summary>
		/// Submits all changes.
		/// </summary>
		public void EndEdit()
		{
			if( m_backupStream != null )
			{
				m_backupStream.Close();
				m_backupStream = null;
			}
		}
		#endregion
	}
}