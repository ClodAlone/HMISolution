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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Base class for standard controls in the HTML.
    /// </summary>
    public abstract class UserControlImpl
      : ControlBase, IControlImpl
    {
        #region Class members
        /// <summary>
        /// Indicates whether to set location to the control.
        /// </summary>
        private bool m_bSetLocation;

        /// <summary>
        /// Indicates whether the custom control needs to be disposed.
        /// </summary>
        private bool m_bNeedDispose = true;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the parent tag element of the user control class.
        /// </summary>
        public new IUserControlHolder Parent
        {
            get
            {
                return base.Parent as IUserControlHolder;
            }
        }

        /// <summary>
        /// Gets the collection of control elements in the document.
        /// </summary>
        protected internal ArrayList DocumentUserControls
        {
            get
            {
                return this.ParentElement.Document.UserControls;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set location.
        /// </summary>
        internal bool LocationSet
        {
            get
            {
                return m_bSetLocation;
            }
            set
            {
                if (m_bSetLocation != value)
                {
                    m_bSetLocation = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control must be disposed.
        /// </summary>
        /// <remarks>Default value is True.</remarks>
        internal bool NeedDispose
        {
            get
            {
                return m_bNeedDispose;
            }
            set
            {
                if (m_bNeedDispose != value)
                {
                    m_bNeedDispose = value;
                }
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the UserControlImpl class
        /// </summary>
        protected internal UserControlImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UserControlImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        protected internal UserControlImpl(IUserControlHolder parent)
            : this(parent, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UserControlImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        /// <param name="bInitialize">If True, raises InitializeControl method.</param>
        protected internal UserControlImpl(IUserControlHolder parent, bool bInitialize)
            : base(parent)
        {
            if (bInitialize) InitializeControl();
        }

        /// <summary>
        /// Overloaded. Disposes control.
        /// </summary>
        public override void DisposeControl()
        {
            base.DisposeControl();

            if (this.Parent != null)
            {
                HTMLAttributesCollection attributes = this.Parent.Attributes as HTMLAttributesCollection;

                if (attributes != null)
                {
                    attributes.Inserted -= new CollectionEventHandler(CollectionChanged);
                    attributes.Removed -= new CollectionEventHandler(CollectionChanged);
                }
            }
        }

        #endregion

        #region Class Public Methods
        /// <summary>
        /// Overridden. Sets the controls to the specified location of their elements.
        /// </summary>
        public override void SetLocation()
        {
            if (this.CustomControl != null && !this.LocationSet)
            {
                Point location = this.Parent.Location;
                location = this.ParentElement.ReduceLocation(location);
                location = this.Parent.Control.VirtualToClient(location);

                this.CustomControl.Location = location;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Configures the user control.
        /// </summary>
        public virtual void ConfigureControl()
        {
            AttachEvents();

            HTMLAttributesCollection attributes = this.Parent.Attributes as HTMLAttributesCollection;

            if (attributes != null)
            {
                attributes.Inserted -= new CollectionEventHandler(CollectionChanged);
                attributes.Removed -= new CollectionEventHandler(CollectionChanged);

                attributes.Inserted += new CollectionEventHandler(CollectionChanged);
                attributes.Removed += new CollectionEventHandler(CollectionChanged);
            }

            SetTabOrder();
            ConfigureFromFormat();

            if (!DocumentUserControls.Contains(this))
            {
                DocumentUserControls.Add(this);
            }

            OnInitEndCallback();
        }

        /// <summary>
        /// Sets the element's Enabled State.
        /// </summary>
        protected virtual void SetEnabledState()
        {
            bool enabled = !ParentElement.Attributes.Contains(AttributeName.Disabled);
            this.CustomControl.Enabled = enabled;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Configures the control from the format of the parent object.
        /// </summary>
        private void ConfigureFromFormat()
        {
            ConfigureBackColor();
            SetEnabledState();
        }

        /// <summary>
        /// Resizes control if width or height were set.
        /// </summary>
        private void ConfigureSize()
        {
            if (this.ParentElement.IsStyleWidth)
            {
                CustomControl.Width = ParentElement.Format.Width;
            }

            if (this.ParentElement.IsStyleHeight)
            {
                CustomControl.Height = ParentElement.Format.Height;
            }
        }

        /// <summary>
        /// Sets the back color to the control.
        /// </summary>
        private void ConfigureBackColor()
        {
            Color backColor = ParentElement.Format.BackgroundColor;

            if (backColor != Color.Empty && backColor == Color.Transparent)
            {
                CustomControl.BackColor = ParentElement.Control.BackColor;
            }
            else
            {
                CustomControl.BackColor = backColor;
            }
            
        }
        #endregion

        #region Class event handlers
        /// <summary>
        /// Handles adding / removing attributes of the element.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void CollectionChanged(object sender, CollectionEventArgs e)
        {
            HTMLAttributeImpl attr = e.Value as HTMLAttributeImpl;

            if (attr != null && Utilities.StrEquals(attr.Name, AttributeName.Disabled))
            {
                SetEnabledState();
            }
        }
        #endregion
    }
}
