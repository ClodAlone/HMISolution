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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Collection of images for ImageListAdv.
	/// </summary>
	[Editor( typeof( ImageCollection.ImageCollectionEditor ), typeof( UITypeEditor ) )]
	[Serializable]
	public class ImageCollection
		: CollectionBase
		, ICollection
		, IList
	{
		#region Classes

		/// <summary>
		/// Custom type descriptor for <see cref="Image"/> class.
		/// <remarks>Filters our <see cref="Image.Tag"/> property</remarks>
		/// </summary>
		class CustomImageTypeDescriptor:
			CustomTypeDescriptor
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="CustomImageTypeDescriptor"/> class.
			/// </summary>
			/// <param name="parent">The parent custom type descriptor.</param>
			public CustomImageTypeDescriptor( ICustomTypeDescriptor parent ) :
				base( parent )
			{
			}

			/// <summary>
			/// Returns a collection of property descriptors for the object represented by this type descriptor.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
			/// </returns>
			public override PropertyDescriptorCollection GetProperties()
			{
				return FilterProperties( base.GetProperties() );
			}

			/// <summary>
			/// Returns a filtered collection of property descriptors for the object represented by this type descriptor.
			/// </summary>
			/// <param name="attributes">An array of attributes to use as a filter. This can be null.</param>
			/// <returns>
			/// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
			/// </returns>
			public override PropertyDescriptorCollection GetProperties( Attribute[] attributes )
			{
				return FilterProperties( base.GetProperties( attributes ) );
			}

			/// <summary>
			/// Filters the properties.
			/// </summary>
			/// <param name="props">The initial collection of property descriptors.</param>
			/// <returns>The filtered collection of property descriptors.</returns>
			private static PropertyDescriptorCollection FilterProperties( PropertyDescriptorCollection props )
			{
				PropertyDescriptor pd = props["Tag"];
				ArrayList newProps = new ArrayList( props );

				newProps.Remove( pd );

				return new PropertyDescriptorCollection( (PropertyDescriptor[])newProps.ToArray( typeof( PropertyDescriptor ) ), true );
			}
		}

		/// <summary>
		/// Custom rpovider of custom type descriptor for <see cref="Image"/> class.
		/// </summary>
		class ImageTypeDescriptionProvider:
			TypeDescriptionProvider
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ImageTypeDescriptionProvider"/> class.
			/// </summary>
			public ImageTypeDescriptionProvider() :
				base( TypeDescriptor.GetProvider( typeof( Image ) ) )
			{
			}

			/// <summary>
			/// Gets a custom type descriptor for the given type and object.
			/// </summary>
			/// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
			/// <param name="instance">An instance of the type. Can be null if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor"/>.</param>
			/// <returns>
			/// An <see cref="T:System.ComponentModel.ICustomTypeDescriptor"/> that can provide metadata for the type.
			/// </returns>
			public override ICustomTypeDescriptor GetTypeDescriptor( Type objectType, object instance )
			{
				ICustomTypeDescriptor baseTypeDesc = base.GetTypeDescriptor( objectType, instance );

				return new CustomImageTypeDescriptor( baseTypeDesc );
			}
		}

		/// <summary>
		/// Editor for ImageCollection.
		/// </summary>
		internal class ImageCollectionEditor
			: CollectionEditor
		{
			#region Fields

			/// <summary>
			/// Static instance of <see cref="ImageEditorAdv"/>.
			/// </summary>
			private static ImageEditorAdv s_editor;
			/// <summary>
			/// Static instance of custom <see cref="TypeDescriptionProvider"/> for <see cref="Image"/>s.
			/// </summary>
			private static TypeDescriptionProvider s_imageTypeDescProvider;

			#endregion

			#region Initialization

			/// <summary>
			/// Initializes the <see cref="ImageCollectionEditor"/> class.
			/// </summary>
			static ImageCollectionEditor()
			{
				s_editor = new ImageEditorAdv();
				s_imageTypeDescProvider = new ImageTypeDescriptionProvider();
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="ImageCollectionEditor"/> class.
			/// </summary>
			/// <param name="type">The type of the collection for this editor to edit.</param>
			public ImageCollectionEditor( Type type )
				: base( type )
			{
			}
			#endregion

			#region Overrides

			/// <summary>
			/// Creates the instance.
			/// </summary>
			/// <param name="type">The type.</param>
			/// <returns></returns>
			protected override object CreateInstance( Type type )
			{
				return s_editor.EditValue( base.Context, null );
			}
#if !( SyncfusionFramework1_1 || SyncfusionFramework1_0 )
			/// <summary>
			/// Returns a list containing the given object
			/// </summary>
			/// <param name="instance">An <see cref="T:System.Collections.ArrayList"/> returned as an object.</param>
			/// <returns>
			/// An <see cref="T:System.Collections.ArrayList"/> which contains the individual objects to be created.
			/// </returns>
			protected override IList GetObjectsFromInstance( object instance )
			// fix for defect related to showing error message when Cancel button is pressed on Open File dialog.
			{
				ArrayList result = null;
				if( instance != null )
				{
					result = new ArrayList();
					if( instance is Image )
					{
						result.Add( instance );
					}
					else if( instance is IList )
					{
						result.AddRange( (ICollection)instance );
					}
				}
				return result;
			}

			/// <summary>
			/// Destroys the specified instance of the object.
			/// </summary>
			/// <param name="instance">The object to destroy.</param>
			protected override void DestroyInstance( object instance )
			// fix for defect related to showing error message when images are removed from list.
			{
				Image img = instance as Image;
				if( img != null )
				{
					ImageListAdv imageList = this.Context.Instance as ImageListAdv;
					if( imageList != null )
					{
						imageList.Images.Remove( img );
					}
				}
				base.DestroyInstance( instance );
			}
#endif
			/// <summary>
			/// Edits the value of the specified object using the specified service provider and context.
			/// </summary>
			/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
			/// <param name="provider">A service provider object through which editing services can be obtained.</param>
			/// <param name="value">The object to edit the value of.</param>
			/// <returns>
			/// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
			/// </returns>
			/// <exception cref="T:System.ComponentModel.Design.CheckoutException">
			/// An attempt to check out a file that is checked into a source code management program did not succeed.
			/// </exception>
			public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
			{
				object editedValue = null;

				try
				{
					TypeDescriptor.AddProvider( s_imageTypeDescProvider, typeof( Image ) );
				}
				finally
				{
					editedValue = base.EditValue( context, provider, value );

					TypeDescriptor.RemoveProvider( s_imageTypeDescProvider, typeof( Image ) );
				}

				return editedValue;
			}

			#endregion
		}

		/// <summary>
		/// Editor for images in ImageListAdv.
		/// </summary>
		internal class ImageEditorAdv
			: ImageEditor
		{
			#region Fields
			/// <summary>
			/// FileDialog for selecting images.
			/// </summary>
			private OpenFileDialog m_fileDialog;
			/// <summary>
			/// Array of image editors of different types. Used in building string of file extensions.
			/// </summary>
			private static Type[] m_imageExtenders;
			#endregion

			#region Initialization
			/// <summary>
			/// Initializes static members.
			/// </summary>
			static ImageEditorAdv()
			{
				m_imageExtenders = new Type[] { typeof( BitmapEditor ), typeof( MetafileEditor ) };
			}
			#endregion

			#region Overrides
			/// <summary>
			/// Edits the specified object value using the edit style provided by GetEditStyle.
			/// </summary>
			/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
			/// <param name="provider">A service provider object through which editing services can be obtained.</param>
			/// <param name="value">An instance of the value being edited.</param>
			/// <returns>The new value of the object. If the value of the object has not changed,
			/// this method should return the same object passed to it.</returns>
			public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
			{
#if !( SyncfusionFramework1_1 || SyncfusionFramework1_0 )
				ArrayList list = new ArrayList();

				if( ( provider != null ) && ( ( (IWindowsFormsEditorService)provider.GetService( typeof( IWindowsFormsEditorService ) ) ) != null ) )
				{
					if( m_fileDialog == null )
					{
						m_fileDialog = new OpenFileDialog();
						m_fileDialog.Multiselect = true;

						string text = CreateFilterEntry( this );
						for( int i = 0; i < m_imageExtenders.Length; i++ )
						{
							ImageEditor o = (ImageEditor)Activator.CreateInstance( m_imageExtenders[i],
								BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null );
							Type type = base.GetType();
							Type type2 = o.GetType();
							if( ( !type.Equals( type2 ) && ( o != null ) ) && type.IsInstanceOfType( o ) )
							{
								text = text + "|" + CreateFilterEntry( o );
							}
						}
						m_fileDialog.Filter = text;
					}
					IntPtr hWndFocused = NativeMethods.GetFocus();
					try
					{
						if( m_fileDialog.ShowDialog() == DialogResult.OK )
						{
							foreach( string fileName in m_fileDialog.FileNames )
							{
								bool bAdded = false;
								if( Path.GetExtension( fileName ) == ".ico" )
								{
									Icon icon;
									try
									{
										icon = new Icon( fileName );
										list.Add( ImageListAdv.IconToImageAlphaCorrect( icon ) );
										bAdded = true;
									}
									catch( ArgumentException )
									{
									}
								}

								if( !bAdded )
								{
									FileStream stream = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
									list.Add( this.LoadFromStream( stream ) );
								}
							}
						}
					}
					finally
					{
						if( hWndFocused != IntPtr.Zero )
						{
							NativeMethods.SetFocus( hWndFocused );
						}
					}
				}
				return list;
#else
				if( ( provider != null ) && ( ( ( IWindowsFormsEditorService )provider.GetService( typeof( IWindowsFormsEditorService ) ) ) != null ) )
				{
					if( m_fileDialog == null )
					{
						m_fileDialog = new OpenFileDialog();
						string text = CreateFilterEntry( this );
						for( int i = 0; i < m_imageExtenders.Length; i++ )
						{
							ImageEditor o = ( ImageEditor )Activator.CreateInstance( m_imageExtenders[ i ],
								BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null );
							Type type = base.GetType();
							Type type2 = o.GetType();
							if( ( !type.Equals( type2 ) && ( o != null ) ) && type.IsInstanceOfType( o ) )
							{
								text = text + "|" + CreateFilterEntry( o );
							}
						}
						m_fileDialog.Filter = text;
					}
					IntPtr hWndFocused = NativeMethods.GetFocus();
					try
					{
						if( m_fileDialog.ShowDialog() == DialogResult.OK )
						{
							if( Path.GetExtension( m_fileDialog.FileName ) == ".ico" )
							{
								value = ImageListAdv.IconToImageAlphaCorrect( new Icon( m_fileDialog.FileName ) );
							}
							else
							{
								FileStream stream = new FileStream( m_fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read );
								value = this.LoadFromStream( stream );
							}
						}
					}
					finally
					{
						if( hWndFocused != IntPtr.Zero )
						{
							NativeMethods.SetFocus( hWndFocused );
						}
					}
				}
				return value;
#endif
			}
			#endregion
		}

		/// <summary>
		/// Class containing info about image: the image itself and string key.
		/// </summary>
		private class ImageInfo
		{
			#region Fields
			/// <summary>
			/// Image.
			/// </summary>
			private Image m_image;
			/// <summary>
			/// Key.
			/// </summary>
			private string m_key;
			#endregion

			#region Properties
			/// <summary>
			/// Gets or sets image.
			/// </summary>
			public Image Image
			{
				get
				{
					return m_image;
				}
				set
				{
					m_image = value;
				}
			}
			/// <summary>
			/// Gets or sets key.
			/// </summary>
			public string Key
			{
				get
				{
					return m_key;
				}
				set
				{
					m_key = value;
				}
			}
			#endregion

			#region Initialization
			/// <summary>
			/// Creates new ImageInfo.
			/// </summary>
			/// <param name="image">Image.</param>
			/// <param name="key">Key.</param>
			public ImageInfo( Image image, string key )
			{
				m_image = image;
				m_key = key;
			}
			#endregion
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets value indicating whether collection is empty.
		/// </summary>
		public bool Empty
		{
			get
			{
				return ( this.Count == 0 );
			}
		}
		/// <summary>
		/// Added for compatibility reasons.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets or sets image at specified index.
		/// </summary>
		/// <param name="index">Index of image.</param>
		/// <returns>Image at specified index</returns>
		public Image this[int index]
		{
			get
			{
				if( index < 0 || index > this.Count - 1 ) throw new ArgumentOutOfRangeException( "index" );

				return ( (ImageInfo)this.InnerList[index] ).Image.Clone() as Image;
			}
			set
			{
				if( index < 0 || index > this.Count - 1 ) throw new ArgumentOutOfRangeException( "index" );
				if( value == null ) throw new ArgumentNullException( "value" );

				( (ImageInfo)this.InnerList[index] ).Image = value;
			}
		}
		/// <summary>
		/// Gets or sets image with specified key.
		/// </summary>
		/// <param name="key">Key of image.</param>
		/// <returns>Image with specified key.</returns>
		public Image this[string key]
		{
			get
			{
				ImageInfo foundInfo = null;
				foreach( ImageInfo info in this )
				{
					if( info.Key == key )
					{
						foundInfo = info;
						break;
					}
				}

				Image result = null;
				if( foundInfo != null )
				{
					result = foundInfo.Image.Clone() as Image;
				}

				return result;
			}
			set
			{
				ImageInfo foundInfo = null;
				foreach( ImageInfo info in this )
				{
					if( info.Key == key )
					{
						foundInfo = info;
						break;
					}
				}

				if( foundInfo != null )
				{
					foundInfo.Image = value;
				}
				else
				{
					this.Add( key, value );
				}
			}
		}
		/// <summary>
		/// Gets collection of keys.
		/// </summary>
		public StringCollection Keys
		{
			get
			{
				StringCollection strings = new StringCollection();
				for( int i = 0; i < this.Count; i++ )
				{
					string s = ( (ImageInfo)this.InnerList[i] ).Key;
					if( s != null )
					{
						strings.Add( s );
					}
				}
				return strings;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds icon to the collection.
		/// </summary>
		/// <param name="value">Icon to add.</param>
		public void Add( Icon value )
		{
			if( value == null ) throw new ArgumentNullException( "value" );

			this.InnerList.Add( new ImageInfo( ImageListAdv.IconToImageAlphaCorrect( value ), null ) );
		}
		/// <summary>
		/// Adds image to the collection.
		/// </summary>
		/// <param name="value">Image to add.</param>
		public void Add( Image value )
		{
			if( value == null ) throw new ArgumentNullException( "value" );

			this.InnerList.Add( new ImageInfo( value, null ) );
		}
		/// <summary>
		/// Checks whether collection contains given image.
		/// </summary>
		/// <param name="image">Image to be checked.</param>
		/// <returns>True if collection contains given image; otherwise false.</returns>
		public bool Contains( Image image )
		{
			if( image == null ) throw new ArgumentNullException( "image" );

			foreach( ImageInfo info in this.InnerList )
			{
				if( info.Image == image )
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Gets index of given image.
		/// </summary>
		/// <param name="image">Image to get index of.</param>
		/// <returns>Index of given image or -1 if image doesn't exist in collection.</returns>
		public int IndexOf( Image image )
		{
			if( image == null ) throw new ArgumentNullException( "image" );

			for( int i = 0; i < this.InnerList.Count; i++ )
			{
				if( ( (ImageInfo)this.InnerList[i] ).Image == image )
				{
					return i;
				}
			}
			return -1;
		}
		/// <summary>
		/// Removes given image from collection.
		/// </summary>
		/// <param name="image">Image to remove.</param>
		public void Remove( Image image )
		{
			if( image == null ) throw new ArgumentNullException( "image" );

			for( int i = 0; i < this.InnerList.Count; i++ )
			{
				if( ( (ImageInfo)this.InnerList[i] ).Image == image )
				{
					this.InnerList.RemoveAt( i );
					break;
				}
			}
		}
		/// <summary>
		/// Adds icon with specified key to collection.
		/// </summary>
		/// <param name="key">Key of the icon.</param>
		/// <param name="icon">Icon to add.</param>
		public void Add( string key, Icon icon )
		{
			this.InnerList.Add( new ImageInfo( ImageListAdv.IconToImageAlphaCorrect( icon ), key ) );
		}
		/// <summary>
		/// Adds image with specified key to collection.
		/// </summary>
		/// <param name="key">Key of the image.</param>
		/// <param name="image">Image to add.</param>
		public void Add( string key, Image image )
		{
			this.InnerList.Add( new ImageInfo( image, key ) );
		}
		/// <summary>
		/// Adds array of images to collection.
		/// </summary>
		/// <param name="images">Array of images to add.</param>
		public void AddRange( Image[] images )
		{
			if( images == null ) throw new ArgumentNullException( "images" );

			foreach( Image image in images )
			{
				this.Add( image );
			}
		}
		/// <summary>
		/// Checks whether given key exists in collection.
		/// </summary>
		/// <param name="key">Key to be checked.</param>
		/// <returns>True if given key exists in collection; otherwise false.</returns>
		public bool ContainsKey( string key )
		{
			if( key == null ) throw new ArgumentNullException( "image" );

			foreach( ImageInfo info in this.InnerList )
			{
				if( info.Key == key )
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Gets enumerator for collection.
		/// </summary>
		/// <returns>Enumerator for collection.</returns>
		public new IEnumerator GetEnumerator()
		{
			Image[] imageArray = new Image[this.Count];
			for( int i = 0; i < imageArray.Length; i++ )
			{
				imageArray[i] = this[i];
			}
			return imageArray.GetEnumerator();
		}
		/// <summary>
		/// Gets index of given key.
		/// </summary>
		/// <param name="key">Key to get index of.</param>
		/// <returns>Index if given key or -1 if key doesn't exist in collection.</returns>
		public int IndexOfKey( string key )
		{
			if( key == null ) throw new ArgumentNullException( "image" );

			for( int i = 0; i < this.InnerList.Count; i++ )
			{
				if( ( (ImageInfo)this.InnerList[i] ).Key == key )
				{
					return i;
				}
			}
			return -1;
		}
		/// <summary>
		/// Removes image with specified key.
		/// </summary>
		/// <param name="key">Key of image to remove.</param>
		public void RemoveByKey( string key )
		{
			if( key == null ) throw new ArgumentNullException( "image" );

			for( int i = 0; i < this.InnerList.Count; i++ )
			{
				if( ( (ImageInfo)this.InnerList[i] ).Key == key )
				{
					this.InnerList.RemoveAt( i );
					break;
				}
			}
		}
		/// <summary>
		/// Sets new name to specified key.
		/// </summary>
		/// <param name="index">Index of key to set new name to.</param>
		/// <param name="name">New name of specified key.</param>
		public void SetKeyName( int index, string name )
		{
			if( name == null ) throw new ArgumentNullException( "name" );

			( (ImageInfo)this.InnerList[index] ).Key = name;
		}
		#endregion

		#region Interfaces Implementation
		/// <summary>
		/// Copies the elements of the ICollection to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="dest">The one-dimensional Array that is the destination of the elements copied from ICollection.
		/// The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		void ICollection.CopyTo( Array dest, int index )
		{
			for( int i = 0; i < this.Count; i++ )
			{
				dest.SetValue( this[i], index++ );
			}
		}
		/// <summary>
		/// Adds an item to the IList.
		/// </summary>
		/// <param name="value">The Object to add to the IList.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		int IList.Add( object value )
		{
			if( !( value is Image ) ) throw new ArgumentException( "value" );

			this.Add( (Image)value );
			return ( this.Count - 1 );
		}
		/// <summary>
		/// Determines whether the IList contains a specific value.
		/// </summary>
		/// <param name="image">The Object to locate in the IList.</param>
		/// <returns>True if the Object is found in the IList; otherwise, false.</returns>
		bool IList.Contains( object image )
		{
			if( image is Image )
			{
				return this.Contains( (Image)image );
			}
			return false;
		}
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if( !( value is Image ) )
				{
					throw new ArgumentException( "value" );
				}

				this[index] = (Image)value;
			}
		}
		/// <summary>
		/// Determines the index of a specific item in the IList.
		/// </summary>
		/// <param name="image">The Object to locate in the IList.</param>
		/// <returns>The index of value if found in the list; otherwise, -1.</returns>
		int IList.IndexOf( object image )
		{
			if( image is Image )
			{
				return this.IndexOf( (Image)image );
			}
			return -1;
		}
		/// <summary>
		/// Inserts an item to the IList at the specified position.
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted.</param>
		/// <param name="value">The Object to insert into the IList.</param>
		void IList.Insert( int index, object value )
		{
			if( !( value is Image ) ) throw new ArgumentException( "value" );

			this.InnerList.Insert( index, new ImageInfo( (Image)value, null ) );
		}
		/// <summary>
		/// Removes the first occurrence of a specific object from the IList.
		/// </summary>
		/// <param name="image">The Object to remove from the IList.</param>
		void IList.Remove( object image )
		{
			if( image is Image )
			{
				this.Remove( (Image)image );
			}
		}
		#endregion
	}
}