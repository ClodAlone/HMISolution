// <copyright file="VisualChildrenCollection.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Collection of visual children.
    /// </summary>
    /// <typeparam name="T">Visual elements Type</typeparam>
    public class VisualChildrenCollection<T> : ObservableCollection<T>
    {
        #region Private members
        /// <summary>
        /// Parent of visual element.
        /// </summary>
        private FrameworkElement m_visualParent;
        #endregion Private members

        #region CLR Getters & Setters
        /// <summary>
        /// Gets the parent of visual element.
        /// </summary>
        public FrameworkElement VisualParent
        {
            get
            {
                return m_visualParent;
            }
        }
        #endregion CLR Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualChildrenCollection{T}"/> class.
        /// </summary>        
        public VisualChildrenCollection() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualChildrenCollection{T}"/> class.
        /// </summary>
        /// <param name="visualParent">Parent of the Visual</param>
        public VisualChildrenCollection(FrameworkElement visualParent)
        {
            m_visualParent = visualParent;
        }
        #endregion Initialization
    }

    /// <summary>
    /// Collection of ranges.
    /// </summary>
    public class RangesCollection : VisualChildrenCollection<RangeBase>
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RangesCollection"/> class.
        /// </summary>
        public RangesCollection() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangesCollection"/> class.
        /// </summary>
        /// <param name="parent">Range's parent</param>
        public RangesCollection(FrameworkElement parent)
            : base(parent)
        {
        }
        #endregion Initialization
    }

    /// <summary>
    /// Collection of pointers.
    /// </summary>
    public class PointersCollection : VisualChildrenCollection<CircularPointer>
    {       
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PointersCollection"/> class.
        /// </summary>
        public PointersCollection()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointersCollection"/> class.
        /// </summary>
        /// <param name="parent">Pointer's parent</param>
        public PointersCollection(FrameworkElement parent)
            : base(parent)
        {
        }
        #endregion Initialization
    }

    /// <summary>
    /// Collection of ticks.
    /// </summary>
    public class TicksCollection : VisualChildrenCollection<TickBase>
    {        
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TicksCollection"/> class.
        /// </summary>
        public TicksCollection() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicksCollection"/> class.
        /// </summary>
        /// <param name="parent">Tick's parent</param>
        public TicksCollection(FrameworkElement parent)
            : base(parent)
        {
        }
        #endregion Initialization
    }

    /// <summary>
    /// Collection of pointers.
    /// </summary>
    public class LinearPointersCollection : VisualChildrenCollection<LinearPointer>
    {        
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearPointersCollection"/> class.
        /// </summary>
        public LinearPointersCollection()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearPointersCollection"/> class.
        /// </summary>
        /// <param name="parent">Pointer's parent</param>
        public LinearPointersCollection(FrameworkElement parent)
            : base(parent)
        {
        }
        #endregion Initialization
    }
}
