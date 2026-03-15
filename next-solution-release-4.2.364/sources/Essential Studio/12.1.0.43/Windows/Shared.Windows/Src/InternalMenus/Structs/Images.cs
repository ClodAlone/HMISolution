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
using System.Collections;
using System.Windows.Forms;
using System.Resources;

namespace Syncfusion.Windows.Forms.InternalMenus
{
    /// <exclude/>
    /// <summary>
	/// Wrapper object that controls the images used in the menu and toolbar implementations
	/// </summary>
	internal class Images
	{
		public static ImageStructCollection imageCollection;
		public static ImageList imageList;
		
		public Images()
		{
		}

		/// <summary>
		///     Adds an image to the internal image list from the internal imageCollection at the specified index
		/// </summary>
		public static void AddToImageList(int collectionIndex)
		{
			if(imageList == null)
			{
				imageList = new ImageList();
			}
			imageList.Images.Add(imageCollection[collectionIndex].bmp);
		}

		public static ImageList CurrentImageList
		{
			get
			{
				return imageList;
			}
		}

		public static ImageStructCollection ImageCollection
		{
			get
			{
				if(imageCollection == null)
				{
					imageCollection = new ImageStructCollection();
				}
				return imageCollection;
			}
		}
	}

    /// <exclude/>
    internal struct ImageStruct
	{
		public System.Drawing.Image bmp;
		public string bmpName;
	}

	




	#region "'ImageStructCollection' strongly typed collection class"


    /// <exclude/>
    /// <summary>
	///     A collection that stores 'ImageStruct' objects.
	/// </summary>
	[Serializable()]
	internal class ImageStructCollection : System.Collections.CollectionBase 
	{
    
		/// <summary>
		///     Initializes a new instance of 'ImageStructCollection'.
		/// </summary>
		public ImageStructCollection() 
		{
		}
    
		/// <summary>
		///     Initializes a new instance of 'ImageStructCollection' based on an already existing instance.
		/// </summary>
		/// <param name='imaValue'>
		///     A 'ImageStructCollection' from which the contents is copied
		/// </param>
		public ImageStructCollection(ImageStructCollection imaValue) 
		{
			this.AddRange(imaValue);
		}
    
		/// <summary>
		///     Initializes a new instance of 'ImageStructCollection' with an array of 'ImageStruct' objects.
		/// </summary>
		/// <param name='imaValue'>
		///     An array of 'ImageStruct' objects with which to initialize the collection
		/// </param>
		public ImageStructCollection(ImageStruct[] imaValue) 
		{
			this.AddRange(imaValue);
		}
    
		/// <summary>
		///     Represents the 'ImageStruct' item at the specified index position.
		/// </summary>
		/// <param name='intIndex'>
		///     The zero-based index of the entry to locate in the collection.
		/// </param>
		/// <value>
		///     The entry at the specified index of the collection.
		/// </value>
		public ImageStruct this[int intIndex] 
		{
			get 
			{
				return ((ImageStruct)(List[intIndex]));
			}
			set 
			{
				List[intIndex] = value;
			}
		}
    
		/// <summary>
		///     Adds a 'ImageStruct' item with the specified value to the 'ImageStructCollection'
		/// </summary>
		/// <param name='imaValue'>
		///     The 'ImageStruct' to add.
		/// </param>
		/// <returns>
		///     The index at which the new element was inserted.
		/// </returns>
		public int Add(ImageStruct imaValue) 
		{
			return List.Add(imaValue);
		}
    
		/// <summary>
		///     Copies the elements of an array at the end of this instance of 'ImageStructCollection'.
		/// </summary>
		/// <param name='imaValue'>
		///     An array of 'ImageStruct' objects to add to the collection.
		/// </param>
		public void AddRange(ImageStruct[] imaValue) 
		{
			for (int intCounter = 0; (intCounter < imaValue.Length); intCounter = (intCounter + 1)) 
			{
				this.Add(imaValue[intCounter]);
			}
		}
    
		/// <summary>
		///     Adds the contents of another 'ImageStructCollection' at the end of this instance.
		/// </summary>
		/// <param name='imaValue'>
		///     A 'ImageStructCollection' containing the objects to add to the collection.
		/// </param>
		public void AddRange(ImageStructCollection imaValue) 
		{
			for (int intCounter = 0; (intCounter < imaValue.Count); intCounter = (intCounter + 1)) 
			{
				this.Add(imaValue[intCounter]);
			}
		}
    
		/// <summary>
		///     Gets a value indicating whether the 'ImageStructCollection' contains the specified value.
		/// </summary>
		/// <param name='imaValue'>
		///     The item to locate.
		/// </param>
		/// <returns>
		///     True if the item exists in the collection; false otherwise.
		/// </returns>
		public bool Contains(ImageStruct imaValue) 
		{
			return List.Contains(imaValue);
		}
    
		/// <summary>
		///     Gets a value indicating the index within 'ImageStructCollection' of an ImageStruct with the specified resource name.
		/// </summary>
		/// <param name='resourceName'>
		///     The value to locate.
		/// </param>
		/// <param name="addIfNotFound">
		///		True to add the image if it does not exist
		/// </param>
		/// <param name="manager">
		///		The Resource Manager that contains the image
		/// </param>
		/// <returns>
		///     True if the item exists in the collection; false otherwise.
		/// </returns>
		public int IndexOf(string resourceName,bool addIfNotFound,ResourceManager manager) 
		{
			ImageStructCollection.ImageStructEnumerator ienum = this.GetEnumerator();
			while(ienum.MoveNext())
			{
				if(ienum.Current.bmpName == resourceName)
				{
					return this.IndexOf(ienum.Current);
				}
			}
			if(addIfNotFound)
			{
				ImageStruct iStruct = new ImageStruct();
				object obj = manager.GetObject(resourceName);
				//if resource is an Icon, create a bmp from it
				if(obj.GetType() == typeof(System.Drawing.Icon))
				{
					iStruct.bmp = Bitmap.FromHicon(((Icon)obj).Handle);
				}
				else
				{
					iStruct.bmp = manager.GetObject(resourceName) as Image;
					((Bitmap)iStruct.bmp).MakeTransparent(((Bitmap)iStruct.bmp).GetPixel(0, 0));
				}
				iStruct.bmpName = resourceName;
				int iconIndex = Add(iStruct);
				Images.AddToImageList(iconIndex);
				return iconIndex;
			}
			return -1;
		}

		/// <summary>
		///     Copies the 'ImageStructCollection' values to a one-dimensional System.Array
		///     instance starting at the specified array index.
		/// </summary>
		/// <param name='imaArray'>
		///     The one-dimensional System.Array that represents the copy destination.
		/// </param>
		/// <param name='intIndex'>
		///     The index in the array where copying begins.
		/// </param>
		public void CopyTo(ImageStruct[] imaArray, int intIndex) 
		{
			List.CopyTo(imaArray, intIndex);
		}
    
		/// <summary>
		///     Returns the index of a 'ImageStruct' object in the collection.
		/// </summary>
		/// <param name='imaValue'>
		///     The 'ImageStruct' object whose index will be retrieved.
		/// </param>
		/// <returns>
		///     If found, the index of the value; otherwise, -1.
		/// </returns>
		public int IndexOf(ImageStruct imaValue) 
		{
			return List.IndexOf(imaValue);
		}
    
		/// <summary>
		///     Inserts an existing 'ImageStruct' into the collection at the specified index.
		/// </summary>
		/// <param name='intIndex'>
		///     The zero-based index where the new item should be inserted.
		/// </param>
		/// <param name='imaValue'>
		///     The item to insert.
		/// </param>
		public void Insert(int intIndex, ImageStruct imaValue) 
		{
			List.Insert(intIndex, imaValue);
		}
    
		/// <summary>
		///     Returns an enumerator that can be used to iterate through
		///     the 'ImageStructCollection'.
		/// </summary>
		public new ImageStructEnumerator GetEnumerator() 
		{
			return new ImageStructEnumerator(this);
		}
    
		/// <summary>
		///     Removes a specific item from the 'ImageStructCollection'.
		/// </summary>
		/// <param name='imaValue'>
		///     The item to remove from the 'ImageStructCollection'.
		/// </param>
		public void Remove(ImageStruct imaValue) 
		{
			List.Remove(imaValue);
		}

        /// <exclude/>
        /// <summary>
		///     A strongly typed enumerator for 'ImageStructCollection'
		/// </summary>
		internal class ImageStructEnumerator : object, System.Collections.IEnumerator 
		{
        
			private System.Collections.IEnumerator iEnBase;
        
			private System.Collections.IEnumerable iEnLocal;
        
			/// <summary>
			///     Enumerator constructor
			/// </summary>
			public ImageStructEnumerator(ImageStructCollection imaMappings) 
			{
				this.iEnLocal = ((System.Collections.IEnumerable)(imaMappings));
				this.iEnBase = iEnLocal.GetEnumerator();
			}
        
			/// <summary>
			///     Gets the current element from the collection (strongly typed)
			/// </summary>
			public ImageStruct Current 
			{
				get 
				{
					return ((ImageStruct)(iEnBase.Current));
				}
			}
        
			/// <summary>
			///     Gets the current element from the collection
			/// </summary>
			object System.Collections.IEnumerator.Current 
			{
				get 
				{
					return iEnBase.Current;
				}
			}
        
			/// <summary>
			///     Advances the enumerator to the next element of the collection
			/// </summary>
			public bool MoveNext() 
			{
				return iEnBase.MoveNext();
			}
        
			/// <summary>
			///     Advances the enumerator to the next element of the collection
			/// </summary>
			bool System.Collections.IEnumerator.MoveNext() 
			{
				return iEnBase.MoveNext();
			}
        
			/// <summary>
			///     Sets the enumerator to the first element in the collection
			/// </summary>
			public void Reset() 
			{
				iEnBase.Reset();
			}
        
			/// <summary>
			///     Sets the enumerator to the first element in the collection
			/// </summary>
			void System.Collections.IEnumerator.Reset() 
			{
				iEnBase.Reset();
			}
		}
	}

	#endregion //('ImageStructCollection' strongly typed collection class)

}
