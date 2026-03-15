#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Xml.Serialization;

using Syncfusion.XmlSerializersCreator;

namespace Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets
{
	/// <summary>
	/// Class containing code snippets and inner code snippets containers.
	/// </summary>
	public class CodeSnippetsContainer
		: IXmlSerializable
		, IEnumerable
	{
		#region Constants
		/// <summary>
		/// Text of ambiguous container name exception.
		/// </summary>
		private const string DEF_CONTAINERS_EXCEPTION = "Names of containers must be unique within parent container";
		/// <summary>
		/// Text of ambiguous snippet title exception.
		/// </summary>
		private const string DEF_SNIPPETS_EXCEPTION = "Titles of snippets must be unique within parent container";
		#endregion

		#region Fields
		/// <summary>
		/// List of snippets.
		/// </summary>
		private ArrayList m_snippets;
		/// <summary>
		/// List of inner containers.
		/// </summary>
		private ArrayList m_containers;
		/// <summary>
		/// Name of container.
		/// </summary>
		private string m_name = string.Empty;
		/// <summary>
		/// List of names of code snippets.
		/// </summary>
		private ArrayList m_snippetsNames;
		/// <summary>
		/// List of names of inner containers.
		/// </summary>
		private ArrayList m_containersNames;
		/// <summary>
		/// Parent container.
		/// </summary>
		private CodeSnippetsContainer m_parent;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets name of container.
		/// </summary>
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "Name" );

				if( m_name != value )
				{
					if( NameChanging != null )
					{
						NameChanging( this, new ValueChangedEventArgs( m_name, value ) );
					}

					m_name = value;
				}
			}
		}
		/// <summary>
		/// Returns value indicating whether container is empty.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return ( m_snippets.Count == 0 && m_containers.Count == 0 );
			}
		}
		/// <summary>
		/// Gets names of code snippets.
		/// </summary>
		public ArrayList SnippetsNames
		{
			get
			{
				if( m_snippetsNames == null )
				{
					m_snippetsNames = new ArrayList();

					foreach( CodeSnippet snippet in m_snippets )
					{
						m_snippetsNames.Add( snippet.Title );
					}
				}

				return m_snippetsNames;
			}
		}
		/// <summary>
		/// Gets names of inner containers.
		/// </summary>
		public ArrayList ContainersNames
		{
			get
			{
				if( m_containersNames == null )
				{
					m_containersNames = new ArrayList();

					foreach( CodeSnippetsContainer container in m_containers )
					{
						m_containersNames.Add( container.Name );
					}
				}

				return m_containersNames;
			}
		}
		/// <summary>
		/// Gets number of containers.
		/// </summary>
		public int ContainersNumber
		{
			get
			{
				return m_containers.Count;
			}
		}
		#endregion

		#region Internal Properties
		/// <summary>
		/// Gets or sets parent container.
		/// </summary>
		internal CodeSnippetsContainer Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				m_parent = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of CodeSnippetsContainer.
		/// </summary>
		public CodeSnippetsContainer()
		{
			m_snippets = new ArrayList();
			m_containers = new ArrayList();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets code snippet by it's title.
		/// </summary>
		/// <param name="title">Title of code snippet that has to be found.</param>
		/// <returns>Needed code snippet or null if there's no snippet with given title.</returns>
		public CodeSnippet GetSnippetByTitle( string title )
		{
			if( title == null || title == string.Empty ) throw new ArgumentNullException( "title" );

			CodeSnippet result = null;

			foreach( CodeSnippet snippet in m_snippets )
			{
				if( snippet.Title == title ) result = snippet;
			}

			return result;
		}
		/// <summary>
		/// Gets code snippet by it's shortcut.
		/// </summary>
		/// <param name="shortcut">Shortcut of code snippet that has to be found.</param>
		/// <returns>Needed code snippet or null if there's no snippet with given shortcut.</returns>
		public CodeSnippet GetSnippetByShortcut( string shortcut )
		{
			if( shortcut == null || shortcut == string.Empty ) throw new ArgumentNullException( "shortcut" );

			CodeSnippet result = null;

			foreach( CodeSnippet snippet in m_snippets )
			{
				if( snippet.Shortcut == shortcut ) result = snippet;
			}

			if( result == null )
			{
				foreach( CodeSnippetsContainer container in m_containers )
				{
					result = container.GetSnippetByShortcut( shortcut );
				}
			}

			return result;
		}
		/// <summary>
		/// Gets inner container by it's name.
		/// </summary>
		/// <param name="name">Name of container.</param>
		/// <returns>Found container or null is nothing was found.</returns>
		public CodeSnippetsContainer GetContainerByName( string name )
		{
			if( name == null || name == string.Empty ) throw new ArgumentNullException( "name" );

			CodeSnippetsContainer result = null;

			foreach( CodeSnippetsContainer container in m_containers )
			{
				if( container.Name == name ) result = container;
			}

			return result;
		}
		/// <summary>
		/// Adds new code snippet to the collection.
		/// </summary>
		/// <param name="snippet">Code snippet to add.</param>
		public void AddSnippet( CodeSnippet snippet )
		{
			if( this.SnippetsNames.Contains( snippet.Title ) )
				throw new ArgumentException( DEF_SNIPPETS_EXCEPTION );

			snippet.TitleChanging += new ValueChangedEventHandler( snippet_TitleChanging );
			m_snippets.Add( snippet );
			ResetSnippetsNames();
		}
		/// <summary>
		/// Adds new inner container to the collection.
		/// </summary>
		/// <param name="container">Container to add.</param>
		public void AddContainer( CodeSnippetsContainer container )
		{
			if( this.ContainersNames.Contains( container.Name ) )
				throw new ArgumentException( DEF_CONTAINERS_EXCEPTION );

			container.Parent = this;
			container.NameChanging += new ValueChangedEventHandler( container_NameChanging );
			m_containers.Add( container );
			ResetContainersNames();
		}
		/// <summary>
		/// Removes container with given name from collection of inner containers.
		/// </summary>
		/// <param name="name">Name of container to remove.</param>
		public void RemoveContainer( string name )
		{
			CodeSnippetsContainer container = GetContainerByName( name );

			if( container != null )
			{
				container.Parent = null;
				container.NameChanging -= new ValueChangedEventHandler( container_NameChanging );
				m_containers.Remove( container );
				ResetContainersNames();
			}
		}
		/// <summary>
		/// Removes snippet with given title from collection of code snippets.
		/// </summary>
		/// <param name="title">Title of snippet to remove.</param>
		public void RemoveSnippet( string title )
		{
			CodeSnippet snippet = GetSnippetByTitle( title );

			if( snippet != null )
			{
				snippet.TitleChanging -= new ValueChangedEventHandler( snippet_TitleChanging );
				m_snippets.Remove( snippet );
				ResetSnippetsNames();
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Raised when name of the container is going to be changed.
		/// </summary>
		public event ValueChangedEventHandler NameChanging;
		#endregion

		#region Event Handlers
		/// <summary>
		/// Checks for containers names ambiguity.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void container_NameChanging( object sender, ValueChangedEventArgs e )
		{
			if( this.ContainersNames.Contains( ( string )e.newValue ) ) throw new Exception( DEF_CONTAINERS_EXCEPTION );

			ResetContainersNames();
		}
		/// <summary>
		/// Checks for snippets names ambiguity.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void snippet_TitleChanging( object sender, ValueChangedEventArgs e )
		{
			if( this.SnippetsNames.Contains( ( string )e.newValue ) ) throw new Exception( DEF_SNIPPETS_EXCEPTION );

			ResetSnippetsNames();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Resets collection of snippets names.
		/// </summary>
		private void ResetSnippetsNames()
		{
			m_snippetsNames = null;
		}
		/// <summary>
		/// Resets collection of inner containers names.
		/// </summary>
		private void ResetContainersNames()
		{
			m_containersNames = null;
		}
		#endregion

		#region IXmlSerializable Members
		/// <summary>
		/// Not used. For proper interface implementation.
		/// </summary>
		/// <returns>XmlSchema.</returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}
		/// <summary>
		/// Custom Xml deserialization.
		/// </summary>
		/// <param name="reader">XmlReader.</param>
		public void ReadXml( System.Xml.XmlReader reader )
		{
			if( null == reader )
				throw new ArgumentNullException( "reader" );

			if( reader.MoveToAttribute( "Name" ) )
			{
				reader.ReadAttributeValue();
				this.Name = reader.Value;
				reader.MoveToElement();
			}

			reader.ReadStartElement( "CodeSnippetsContainer" );

			if( !reader.IsEmptyElement )
			{
				XmlSerializer containerSer = SerializersManager.GetSerializer( typeof( CodeSnippetsContainer ) );

				while( reader.Name == "CodeSnippetsContainer" )
				{
					CodeSnippetsContainer container = ( CodeSnippetsContainer )containerSer.Deserialize( reader );
					AddContainer( container );
				}

				XmlSerializer snippetSer = SerializersManager.GetSerializer( typeof( CodeSnippet ) );

				while( reader.Name == "CodeSnippet" )
				{
					CodeSnippet snippet = ( CodeSnippet )snippetSer.Deserialize( reader );
					AddSnippet( snippet );
				}
			}

			reader.ReadEndElement();
		}
		/// <summary>
		/// Custom Xml serialization.
		/// </summary>
		/// <param name="writer">XmlWriter.</param>
		public void WriteXml( System.Xml.XmlWriter writer )
		{
			if( m_name != string.Empty )
			{
				writer.WriteAttributeString( "Name", m_name );
			}

			// Containers.
			XmlSerializer valueSer = SerializersManager.GetSerializer( typeof( CodeSnippetsContainer ) );
			foreach( CodeSnippetsContainer container in m_containers )
			{
				valueSer.Serialize( writer, container );
			}

			// Code snippets.
			valueSer = SerializersManager.GetSerializer( typeof( CodeSnippet ) );
			foreach( CodeSnippet snippet in m_snippets )
			{
				valueSer.Serialize( writer, snippet, null );
			}
		}
		#endregion

		#region IEnumerable Members
		/// <summary>
		/// Gets enumerator for collection of snippets.
		/// </summary>
		/// <returns>IEnumerator.</returns>
		public IEnumerator GetEnumerator()
		{
			return m_snippets.GetEnumerator();
		}
		#endregion
	}
}