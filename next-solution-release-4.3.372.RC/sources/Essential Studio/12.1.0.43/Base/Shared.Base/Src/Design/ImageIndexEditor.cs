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
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using Syncfusion;
using Syncfusion.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Collections;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Design
{
	// Use this to make your ImageIndex properties act like the WinForms ImageIndex properties.
	// Usage Example:
	//	[
	//	DefaultValue(-1),
	//	Category("Appearance"),
	//	TypeConverter(typeof(System.Windows.Forms.ImageIndexConverter)),
	//	Editor(typeof(ImageIndexEditor), typeof(UITypeEditor))
	//	]
	//	public virtual int ImageIndex
	//
	// The Converter and the Editor rely on the instance with the ImageIndex property (or
	// one of its parent) having an "ImageList" property. Parents are obtained through
	// a "Parent" property (returning something that could be cast to object), if available.

	[Syncfusion.Documentation.DocumentationExclude()]
	public class ImageIndexEditor : UITypeEditor
	{
		// Fields
		private ImageList currentImageList;
		private object currentInstance;
		private UITypeEditor imageEditor;

		public const string DEF_IMGLIST_PROP_NAME = @"ImageList";
		internal static readonly string[] s_aImgListPropNames = new string[] { DEF_IMGLIST_PROP_NAME };

		// Constructors
		public ImageIndexEditor()
		{
			this.imageEditor = (System.Drawing.Design.UITypeEditor)TypeDescriptor.GetEditor(typeof(System.Drawing.Image),typeof(System.Drawing.Design.UITypeEditor));
		}

		#region Properties
			protected virtual string[] ImageListPropertyNames
			{
				get
				{
					return s_aImgListPropNames;
				}
			}
		#endregion

		// Methods
		protected virtual bool IsPropertyValidForInstance( object instance, string sProperty )
		{
			Type type = instance.GetType();
			PropertyInfo pi = type.GetProperty( sProperty, typeof(ImageList) );

			return null != pi;
		}

		public override /*UITypeEditor*/ void PaintValue(PaintValueEventArgs e)
		{
			System.Drawing.Image image;
			if (this.imageEditor != null)
			{
				if(e.Value is System.Int32)
				{
					image = this.GetImage(e.Context,((int) e.Value));
					if (image != null)
						this.imageEditor.PaintValue(new PaintValueEventArgs(e.Context,image,e.Graphics,e.Bounds));
				}
			}
		}

		public override /*UITypeEditor*/ bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			if (this.imageEditor != null)
				return this.imageEditor.GetPaintValueSupported(context);
			else
				return false;
		}

		protected virtual object GetParentInstance( object instance, PropertyDescriptorCollection propertyDescriptorCollection )
		{
			PropertyDescriptor pd = propertyDescriptorCollection[(string)@"Parent"];

			return (pd != null) ? pd.GetValue(instance) : null;
		}

		private Image GetImage(ITypeDescriptorContext context, int index)
		{
			Image image = null;
			object instance;
			PropertyDescriptor imageListPropDesc;
			PropertyDescriptorCollection propertyDescriptorCollection;

			instance = context.Instance;
			if (index < 0) return image;

			//if (instance != this.currentInstance)
			{
				this.currentInstance = instance;

				imageListPropDesc = null;

				bool bExitLoop = false;

				while( !bExitLoop && null != instance )
				{
					// Check if the instance has a ImageList Property
					propertyDescriptorCollection = TypeDescriptor.GetProperties(instance);

					foreach( string sImageListPropertyName in this.ImageListPropertyNames )
					{
						if( IsPropertyValidForInstance( instance, sImageListPropertyName ) )
						{
							PropertyDescriptor pdImageList = propertyDescriptorCollection[sImageListPropertyName];
							if(pdImageList != null && typeof(ImageList).IsAssignableFrom(pdImageList.PropertyType))
							{
								imageListPropDesc = pdImageList;
							}
							else
							{
								foreach(PropertyDescriptor pd in propertyDescriptorCollection)
								{
									if (typeof(ImageList).IsAssignableFrom(pd.PropertyType))
										imageListPropDesc = pd;
								}
							}
							// If can't find the ImageList property look for it in the Parent
                            if (imageListPropDesc != null)
							{
								this.currentImageList = (ImageList)imageListPropDesc.GetValue(instance);
                                bExitLoop = null != this.currentImageList;

								if( bExitLoop )
								{
									break;
								}
							}
						}
					}

					if( !bExitLoop )
					{
						instance = GetParentInstance( instance, propertyDescriptorCollection );
					}
				}
			}

			if (this.currentImageList != null)
			{
				if (index < this.currentImageList.Images.Count)
					image = this.currentImageList.Images[index];
			}
			return image;
		}
	}
}