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
using System.Drawing;
using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Summary description for NamedImage.
	/// </summary>
	internal class NamedImage
    : INamedImage
	{
    #region Class Private Members
    /// <summary>
    /// Name of the image.
    /// </summary>
    private string m_sName;
    /// <summary>
    /// Image itself.
    /// </summary>
    private Image m_imgImage;
    /// <summary>
    /// Image collection.
    /// </summary>
    private NamedImageList m_parent;
    /// <summary>
    /// Specifies transparent color of the image.
    /// </summary>
    private Color m_clTransparent;
    #endregion

    #region Class Initialization
    /// <summary>
    /// Creates and initalizes new instance of the named image.
    /// </summary>
    /// <param name="parent">Parent collection.</param>
    /// <param name="name">Image name, can not be null.</param>
    /// <param name="image">Image itself.</param>
    /// <param name="transparent">Transparent color.</param>
		public NamedImage( NamedImageList parent, string name, Image image, Color transparent )
		{
      if( null == parent )
        throw new ArgumentNullException( "parent" );

      if( string.Empty == name )
        throw new ArgumentNullException( "name", "Name can not be null." );

      if( null == image )
        throw new ArgumentNullException( "image" );

      m_sName = name;
      m_imgImage = image;
      m_parent = parent;
      m_clTransparent = transparent;
		}
    #endregion
	
    #region Class Properties
    /// <summary>
    /// Gets name of the image.
    /// </summary>
    public string Name
    {
      get
      {
        return m_sName;
      }
    }
    /// <summary>
    /// Gets image.
    /// </summary>
    public Image Image
    {
      get
      {
        return m_imgImage;
      }
    }
    /// <summary>
    /// Gets transparent color of the image.
    /// </summary>
    public Color TransparentColor
    {
      get
      {
        return m_clTransparent;
      }
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Deletes image from collection.
    /// </summary>
    public void Delete()
    {
      throw new NotImplementedException(  );
    }
    #endregion
  }
}
