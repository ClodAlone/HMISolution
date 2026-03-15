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

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Collection of Image instances.
    /// </summary>
    public class ChartImageCollection : CollectionBase
    {
        #region Class properties
        /// <summary>
        /// Gets image by specified index in collection.
        /// </summary>
        /// <value>The position into which the new element was inserted.</value>
        public Image this[int index]
        {
            get
            {
                return this.List[index] as Image;
            }
        }
        #endregion

        #region Class initialize methods
        /// <summary>
        /// Initializes a new instance of the ChartImageCollection.
        /// </summary>
        public ChartImageCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChartImageCollection with collection of images.
        /// </summary>
        /// <param name="source">Collection of images to copy to array.</param>
        public ChartImageCollection(IList source)
        {
            foreach (Image img in source)
            {
                if (img != null)
                {
                    Add(img);
                }
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// At this moment all images in this collection is Bitmaps 
        /// and this color is transparent color.
        /// </summary>
        /// <param name="color">Color to make transparent.</param>
        public void MakeTransparent(Color color)
        {
            for (int i = 0, c = Count; i < c; i++)
            {
                Image img = this[i];

                if (img is Bitmap)
                {
                    Bitmap bmp = img as Bitmap;
                    bmp.MakeTransparent(color);
                }
                else
                {
                    Bitmap bmp = new Bitmap(img);
                    bmp.MakeTransparent(color);
                    this.List[i] = bmp;
                }
            }
        }

        /// <summary>
        /// Add image to collection.
        /// </summary>
        /// <param name="image">Image to add.</param>
        /// <returns>Index of added image.</returns>
        public int Add(Image image)
        {
            return this.List.Add(image);
        }

        /// <summary>
        /// Adds image from a list to the collection.
        /// </summary>
        /// <param name="list">The list.</param>
        public void AddFromList(IList list)
        {
            foreach (Image img in list)
            {
                if (img != null)
                {
                    Add(img);
                }
            }
        }

        /// <summary>
        /// Adds an array of images to this collection.
        /// </summary>        
        public void AddRange(Image[] image)
        {
            this.InnerList.AddRange(image);
        }

        /// <summary>
        /// Removes the image from the list.
        /// </summary>
        /// <param name="image">Image to remove.</param>
        public void Remove(Image image)
        {
            this.List.Remove(image);
        }

        /// <summary>
        /// Check collection to constrains image.
        /// </summary>
        /// <param name="image">Image to check.</param>
        /// <returns>True if collection constrains image, otherwise false.</returns>
        public bool Contains(Image image)
        {
            return IndexOf(image) != -1;
        }

        /// <summary>
        /// Returns the index of the specified image.
        /// </summary>
        /// <param name="image">Image to get index.</param>
        /// <returns>Index of given image. If collection doesn't constrains image return -1.</returns>
        public int IndexOf(Image image)
        {
            return this.List.IndexOf(image);
        }

        /// <summary>
        /// Inserts the image at the specified index.
        /// </summary>
        /// <param name="index">Index to insert image.</param>
        /// <param name="image">Image to insert.</param>
        public void Insert(int index, Image image)
        {
            this.List.Insert(index, image);
        }

        /// <summary>
        /// Copies the images to a one dimensional array starting at the specified index of the target array.
        /// </summary>
        /// <param name="images">Destination array where the elements are to be stored.</param>
        /// <param name="index">Index value from which copy is to start.</param>
        public void CopyTo(Image[] images, int index)
        {
            this.List.CopyTo(images, index);
        }
        #endregion
    }
}
