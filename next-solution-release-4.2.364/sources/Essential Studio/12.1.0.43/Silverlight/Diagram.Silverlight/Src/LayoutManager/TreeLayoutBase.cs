#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Abstract base class providing convenience methods for tree layout algorithms.
    /// </summary>
    public abstract class TreeLayoutBase : LayoutBase
    {
        /// <summary>
        /// The root of the tree
        /// </summary>
        private IShape mlayoutRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TreeLayoutBase"/> class.
        /// </summary>
        /// <param name="model">DiagramModel instance.</param>
        /// <param name="view">DiagramView instance</param>
        protected TreeLayoutBase(DiagramModel model, DiagramView view)
            : base(model, view)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeLayoutBase"/> class.
        /// </summary>
        /// <param name="model">The model instance.</param>
        protected TreeLayoutBase(DiagramModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Gets or sets the layout root.
        /// </summary>
        /// <value>The layout root.</value>
        public IShape LayoutRoot
        {
            get
            {
                return this.mlayoutRoot;
            }

            set
            {
                this.mlayoutRoot = value;
            }
        }
        #region Fields

        #endregion

        #region Consstructor

        #endregion

        #region Properties

        #endregion
    }
}
