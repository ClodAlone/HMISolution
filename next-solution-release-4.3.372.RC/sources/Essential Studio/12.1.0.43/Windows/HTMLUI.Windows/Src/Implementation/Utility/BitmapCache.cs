#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Xml;
using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Image cache class. Holds all images in the document.
    /// </summary>
    internal sealed class BitmapCache : IDisposable
    {
        #region Class members
        /// <summary>
        /// Holder of images, full path as key and bitmap as value.
        /// </summary>
        private Hashtable m_container;

        /// <summary>
        /// Holds the streams from which images were loaded open during
        /// all images lifecycle.
        /// </summary>
        private Hashtable m_streamContainer;

        /// <summary>
        /// Parent document which holds images.
        /// </summary>
        private InputHTML m_document;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the bitmap to cache, full path as key and bitmap as value.
        /// </summary>
        /// <param name="key">String key value</param>
        public Bitmap this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (key.Length == 0)
                    throw new ArgumentException("key - string cannot be empty");

                return (Bitmap)m_container[key];
            }
        }

        /// <summary>
        /// Gets the holder of the open streams containing the image data.
        /// </summary>
        private Hashtable StreamContainer
        {
            get
            {
                if (m_streamContainer == null)
                {
                    m_streamContainer = new Hashtable();
                }

                return m_streamContainer;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the BitmapCache class from being created
        /// </summary>
        private BitmapCache()
        {
            m_container = new Hashtable();
        }

        /// <summary>
        /// Initializes a new instance of the BitmapCache class
        /// </summary>
        /// <param name="document">Holder of images.</param>
        public BitmapCache(InputHTML document)
            : this()
        {
            if (document == null)
                throw new ArgumentNullException("document");

            m_document = document;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Overloaded. Indicates whether such image exists in the cache.
        /// </summary>
        /// <param name="path">Full path to image.</param>
        /// <returns>True if it exists; false otherwise.</returns>
        public bool Contains(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("path - string cannot be empty");

            return m_container.ContainsKey(path);
        }

        /// <summary>
        /// Indicates whether cache holds such bitmap.
        /// </summary>
        /// <param name="image">Image object.</param>
        /// <returns>True if image was inserted into cache or image already exists; False if image is not found.</returns>
        public bool Contains(Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException("image");           
            return m_container.ContainsValue(image);
        }

        /// <summary>
        /// Overloaded. Inserts bitmap which is located by the specified path, to cache.
        /// </summary>
        /// <param name="path">Path to the image.</param>
        /// <returns>True if image was inserted to cache; false otherwise.</returns>
        public bool Insert(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("path - string cannot be empty");

            string fullPath;

            return Insert(path, out fullPath);
        }

        /// <summary>
        /// Inserts bitmap which is located by the specified path, to cache.
        /// </summary>
        /// <param name="path">Path to the image.</param>
        /// <param name="fullPath">Full path to the image.</param>
        /// <returns>True if image was inserted to cache or image already exists; False if image is not found.</returns>
        public bool Insert(string path, out string fullPath)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("path - string cannot be empty");

            bool bInserted = false;
            fullPath = string.Empty;
            ResourceType type = ResourceType.Unknown;
            DataSource dataSource = m_document.DataSource;

            if (dataSource.ResourceExists(path, out fullPath, out type))
            {
                if (!Contains(fullPath))
                {
                    Stream stream = dataSource.GetResource(fullPath, type);

                    // NOTE: don't close stream because animation and other 
                    // features wont be accessible.
                    Bitmap img = Utilities.ImageFromStream(stream);

                    // Check if image has been created.
                    if (img != null)
                    {
                        m_container[fullPath] = img;
                        this.StreamContainer[fullPath] = stream;

                        bInserted = true;
                    }
                    else
                    {
                        // Here we can close stream because image is not loaded.
                        if (stream != null)
                        {
                            stream.Close();
                        }
                    }
                }
                else
                {
                    bInserted = true;
                }
            }

            return bInserted;
        }

        /// <summary>
        /// Inserts image to cache if such key is not in cache.
        /// </summary>
        /// <param name="key">Unique key for cache.</param>
        /// <param name="image">Image for caching.</param>
        /// <returns>True if image was inserted to cache; False if such image already exists in cache.</returns>
        public bool Insert(string key, Bitmap image)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (key.Length == 0)
                throw new ArgumentException("key - string cannot be empty");

            if (image == null)
                throw new ArgumentNullException("image");

            if (!Contains(key))
            {
                m_container[key] = image;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the image from the cache.
        /// </summary>
        /// <param name="path">Path to the image.</param>
        public void Remove(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("path - string cannot be empty");

            Bitmap img = (Bitmap)m_container[path];

            if (img != null)
            {
                m_container.Remove(path);
                img.Dispose();
                img = null;

                // If stream of this image exists - remove it.
                if (m_streamContainer != null)
                {
                    Stream stream = this.StreamContainer[path] as Stream;

                    if (stream != null)
                    {
                        stream.Close();
                        this.StreamContainer.Remove(path);
                    }
                }
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Disposes cache. Removes all images from cache.
        /// </summary>
        public void Dispose()
        {
            if (m_container != null)
            {
                DisposeImages();
                m_container.Clear();
                m_container = null;
            }

            if (m_streamContainer != null)
            {
                CloseStreams();
                m_streamContainer.Clear();
                m_streamContainer = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes all bitmaps in cache.
        /// </summary>
        private void DisposeImages()
        {
            if (m_container == null) return;

            Bitmap bmp;

            foreach (string key in m_container.Keys)
            {
                bmp = m_container[key] as Bitmap;
                if (bmp != null)
                {
                    bmp.Dispose();
                    bmp = null;
                }
            }
        }

        /// <summary>
        /// Closes all open streams.
        /// </summary>
        private void CloseStreams()
        {
            if (m_streamContainer != null)
            {
                Stream stream;

                foreach (string key in m_streamContainer.Keys)
                {
                    stream = m_streamContainer[key] as Stream;

                    if (stream != null)
                    {
                        stream.Close();
                        stream = null;
                    }
                }
            }
        }
        #endregion
    }
}
