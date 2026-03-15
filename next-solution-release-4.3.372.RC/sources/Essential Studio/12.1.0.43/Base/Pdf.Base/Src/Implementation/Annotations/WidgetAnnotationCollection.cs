#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents collection of widget annotations.
    /// </summary>
    internal class WidgetAnnotationCollection : PdfCollection, IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store array of anootation's primitives.
        /// </summary>
        private PdfArray m_array = new PdfArray();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="WidgetAnnotation"/> at the specified index.
        /// </summary>
        /// <value>Annotation at the specified position.</value>
        public WidgetAnnotation this[int index]
        {
            get
            {
                return (WidgetAnnotation)List[index];
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetAnnotationCollection"/> class.
        /// </summary>
        public WidgetAnnotationCollection()
            : base()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the specified annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns></returns>
        public int Add(WidgetAnnotation annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }

            return this.DoAdd(annotation);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="annotation">The annotation.</param>
        public void Insert(int index, WidgetAnnotation annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }

            this.DoInsert(index, annotation);
        }

        /// <summary>
        /// Removes the specified annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public void Remove(WidgetAnnotation annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }

            this.DoRemove(annotation);
        }

        /// <summary>
        /// Removes the annotation at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            this.DoRemoveAt(index);
        }

        /// <summary>
        /// Gets the index of the specified annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns></returns>
        public int IndexOf(WidgetAnnotation annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }

            return List.IndexOf(annotation);
        }

        /// <summary>
        /// Determines whether the annotation is present in collection.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns>
        /// <c>true</c> if collection contains the specified annotation; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(WidgetAnnotation annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }

            return List.Contains(annotation);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            this.DoClear();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns></returns>
        private int DoAdd(WidgetAnnotation annotation)
        {
            this.m_array.Add(new PdfReferenceHolder(annotation));
            return List.Add(annotation);
        }

        /// <summary>
        /// Insters.the annotation at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="annotation">The annotation.</param>
        private void DoInsert(int index, WidgetAnnotation annotation)
        {
            this.m_array.Insert(index, new PdfReferenceHolder(annotation));
            List.Insert(index, annotation);
        }

        /// <summary>
        /// Removes the annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        private void DoRemove(WidgetAnnotation annotation)
        {
            int index = List.IndexOf(annotation);
            this.m_array.RemoveAt(index);
            List.RemoveAt(index);
        }

        /// <summary>
        /// Removes the annotation at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        private void DoRemoveAt(int index)
        {
            this.m_array.RemoveAt(index);
            List.RemoveAt(index);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        private void DoClear()
        {
            this.m_array.Clear();
            List.Clear();
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        Syncfusion.Pdf.Primitives.IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return this.m_array;
            }
        }
        #endregion
    }
}