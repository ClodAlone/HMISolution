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
using System.Drawing;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
  /// <summary>
  /// Collection of the named images.
  /// </summary>
  internal class NamedImageList
    : INamedImagesCollection
  {
    #region Class Private Members
    /// <summary>
    /// Image hashtable.
    /// </summary>
    private Hashtable m_hashImages;
    /// <summary>
    /// Hashtable that stores information about image indexes in different image lists.
    /// </summary>
    private Hashtable m_hashImageLists;
		/// <summary>
		/// Hashtable that contains index-image pairs.
		/// </summary>
		private Hashtable m_hashIndexes;
    #endregion
  
    #region Class Properties
		/// <summary>
		/// Gets named image by it's name.
		/// </summary>
		public INamedImage this[ string index ]
		{
			get
			{
				if( !m_hashImages.Contains( index ) )
					return null;

				return ( INamedImage )m_hashImages[ index ];
			}
		}
		/// <summary>
		/// Gets image by index.
		/// </summary>
		public INamedImage this[ int index ]
		{
			get
			{
				return GetImageByIndex( index );
			}
		}
		/// <summary>
    /// Gets value indicating whether the collection is synchronized.
    /// </summary>
    public bool IsSynchronized
    {
      get
      {
        return m_hashImages.IsSynchronized;
      }
    }

    /// <summary>
    /// Gets count of the images in collection.
    /// </summary>
    public int Count
    {
      get
      {
        return m_hashImages.Count;
      }
    }

    /// <summary>
    /// Gets synchronization root.
    /// </summary>
    public object SyncRoot
    {
      get
      {
        return m_hashImages.SyncRoot;
      }
    }
    #endregion

    #region Class Initialization
    /// <summary>
    /// Creates and initializes new instance of the class.
    /// </summary>
    public NamedImageList()
    {
      m_hashImageLists = new Hashtable();
      m_hashImages = new Hashtable();
			m_hashIndexes = new Hashtable();
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Gets enumerator of the INamedImage objects.
    /// </summary>
    /// <returns>Enumerator.</returns>
    public IEnumerator GetEnumerator()
    {
      return m_hashImages.Values.GetEnumerator();
    }
    /// <summary>
    /// Creates and adds new named image to the collection.
    /// </summary>
    /// <param name="name">Name of the image to be added.</param>
    /// <param name="image">Image to be added.</param>
    /// <returns>INamedImage object.</returns>
    public INamedImage AddImage( string name, Image image )
    {
      return this.AddImage( name, image, Color.Empty );
    }
    /// <summary>
    /// Creates and adds new named image to the collection.
    /// </summary>
    /// <param name="name">Name of the image to be added.</param>
    /// <param name="image">Image to be added.</param>
    /// <param name="transparent">Transparent color of the image.</param>
    /// <returns>INamedImage object.</returns>
    public INamedImage AddImage( string name, Image image, Color transparent )
    {
      NamedImage imageNew = new NamedImage( this, name, image, transparent );

      AddImageInternal( imageNew );
      return imageNew;
    }
    /// <summary>
    /// Adds all images to the image list and stores their indexes for that list.
    /// </summary>
    /// <param name="list">Image list the images are written to.</param>
    public void WriteToImageList( ImageList list )
    {
      Hashtable table = new Hashtable();
      m_hashImageLists[ list ] = table;

      foreach( INamedImage image in this )
      {
        Color transparent = image.TransparentColor;
        int index;

        index = ( transparent.IsEmpty )  
          ? list.Images.Add( image.Image, list.TransparentColor )
          : list.Images.Add( image.Image, transparent );

        table[ image.Name ] = index;
      }
    }
    /// <summary>
    /// Removes information about the image list.
    /// </summary>
    /// <param name="list">Image list.</param>
    public void RemoveImageListInfo( ImageList list )
    {
      m_hashImageLists.Remove( list );
    }
    /// <summary>
    /// Gets index of the named image.
    /// </summary>
    /// <param name="image">Named image.</param>
    /// <param name="list">ImageList.</param>
    /// <returns>Index of the image in the give image list.</returns>
    public int GetIndex( INamedImage image, ImageList list )
    {
      if( null == list )
        throw new ArgumentNullException( "list" );

      if( null == image )
        return -1;

      if( !m_hashImageLists.Contains( list ) )
        throw new ArgumentException( "There is no information related to the given list." );

      Hashtable table = ( Hashtable )m_hashImageLists[ list ];

      int index = ( int )table[ image.Name ];
      return index;
    }
    /// <summary>
    /// Copies data to the specified array.
    /// </summary>
    /// <param name="array">Destanation array.</param>
    /// <param name="index">Starting index in the destination array.</param>
    public void CopyTo( Array array, int index )
    {
      m_hashImages.Values.CopyTo( array, index );
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Ads image to the internal collection.
    /// </summary>
    /// <param name="image">Named image to be added.</param>
    protected void AddImageInternal( NamedImage image )
    {
      if( null == image )
        throw new ArgumentNullException( "image" );

      if( m_hashImages.Contains( image.Name ) )
        throw new ArgumentException( "Image with the given name is already added to collection.", "image" );

      m_hashImages.Add( image.Name, image );
			m_hashIndexes[ m_hashIndexes.Count ] = image;
    }
		/// <summary>
		/// Gets INamedImage by image index.
		/// </summary>
		/// <param name="index">Zero-based index of the image.</param>
		/// <returns>INamedImage or null.</returns>
		internal INamedImage GetImageByIndex( int index )
		{
			if( !m_hashIndexes.Contains( index ) )
			  throw new ArgumentOutOfRangeException( "index", index, "There is no image with such index in collection." );

			INamedImage image = (INamedImage)m_hashIndexes[ index ];
			return image;
		}
		#endregion
	}
}