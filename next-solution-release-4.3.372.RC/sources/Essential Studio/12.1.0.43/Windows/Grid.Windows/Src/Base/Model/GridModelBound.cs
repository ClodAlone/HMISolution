//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelBound.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Abstract base class for objects that are associated with a <see cref="GridModel"/>
    /// </summary>
    public abstract class GridModelBound : NonFinalizeDisposable
    {
        [NonSerialized] internal GridModel model;

        /// <overload>
        /// Initializes <see cref="GridModelBound"/>.
        /// </overload>
        /// <summary>
        /// Initializes <see cref="GridModelBound"/> with a NULL pointer.
        /// </summary>
        protected GridModelBound()
        {
            this.model = null;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            this.model = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// This is called after the model has been deserialized from the <see cref="GridModel"/> implementation of <see cref="IDeserializationCallback.OnDeserialization"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="model">Reference to the  <see cref="GridModel"/> this object is associated with.</param>
        protected virtual void OnModelDeserialization(object sender, GridModel model)
        {
            this.model = model;
        }

        internal void RaiseModelDeserialization(object sender, GridModel model)
        {
            OnModelDeserialization(sender, model);
        }        
    
        /// <summary>
        /// Initializes <see cref="GridModelBound"/> with a reference to a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">Reference to the <see cref="GridModel"/> this object is associated with.</param>
        protected GridModelBound(GridModel model)
        {
            this.model = model;
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetModelInt(GridModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Gets the <see cref="GridModel"/> this object is associated with.
        /// </summary>
        public GridModel Model
        {
            [DebuggerStepThrough()] 
            get { return model; }
        }
    }
}
