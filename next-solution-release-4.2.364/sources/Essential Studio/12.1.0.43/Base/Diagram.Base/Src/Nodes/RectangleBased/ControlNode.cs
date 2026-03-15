#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// This node can be used to host any .NET control inside it.
    /// </summary>
    [Serializable]
    public class ControlNode
        : Node
    {
        #region Class members
        /// <summary>
        /// Snapshot of hosting control
        /// </summary>
        private Image m_ctrlSnapshot;
        /// <summary>
        /// Node activate styles.
        /// </summary>
        private ActivateStyle m_styleActivate;

        /// <summary>
        /// Control Host.
        /// </summary>
        protected Control m_ctrlHosting;

        /// <summary>
        /// Hosting control parent.Needed when Hosting control is activated.
        /// </summary>
        private Control m_ctrlHCParent;

        /// <summary>
        /// Indicates whether Node is activated.
        /// </summary>
        private bool m_bActive;

        /// <summary>
        /// Hash for helper properties.
        /// Hosting Control Properties.
        /// </summary>
        private Hashtable m_hashHCP;
        public bool m_bKeepControlSize;
        public bool m_bExcludeOptimizeContentWithOverlay;
        protected Hashtable m_localSerializedData = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlNode"/> class.
        /// </summary>
        /// <param name="ctrlHosting">The CTRL hosting.</param>
        /// <param name="rectBounds">The rect bounds.</param>
        public ControlNode(Control ctrlHosting, RectangleF rectBounds)
            : this(ctrlHosting, rectBounds, MeasureUnits.Pixel, false, false)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlNode"/> class.
        /// </summary>
        /// <param name="ctrlHosting">The CTRL hosting.</param>
        /// <param name="rectBounds">The rect bounds.</param>
        /// <param name="bKeepControlSize">Keep the control size as control's image size.</param>
        /// <param name="bExcludeOptimizeContentWithOverlay">Exclude the rendering of the control node from the background when OptimizeContentWithOverlay is true.</param>
        /// <Remarks> The param bExcludeOptimizeContentWithOverlay is only applicable for DiagramWebControl. </Remarks>
        public ControlNode(Control ctrlHosting, RectangleF rectBounds, bool bKeepControlSize, bool bExcludeOptimizeContentWithOverlay)
            : this(ctrlHosting, rectBounds, MeasureUnits.Pixel, bKeepControlSize, bExcludeOptimizeContentWithOverlay)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlNode"/> class.
        /// </summary>
        /// <param name="ctrlHosting">The CTRL hosting.</param>
        /// <param name="rectBounds">The rect bounds.</param>
        /// <param name="measureUnits">The measure units.</param>
        /// <param name="bKeepControlSize">Keep the control size as control's image size.</param>
        /// <param name="bExcludeOptimizeContentWithOverlay">Exclude the rendering of the control node from the background when OptimizeContentWithOverlay is true.</param>
        /// <Remarks> The param bExcludeOptimizeContentWithOverlay is only applicable for DiagramWebControl. </Remarks>
        public ControlNode(Control ctrlHosting, RectangleF rectBounds, MeasureUnits measureUnits, bool bKeepControlSize, bool bExcludeOptimizeContentWithOverlay)
        {
            if (ctrlHosting == null)
            {
                throw new ArgumentNullException("ctrlHosting");
            }

            m_styleActivate = ActivateStyle.DoubleClick;
            m_bKeepControlSize = bKeepControlSize;
            m_bExcludeOptimizeContentWithOverlay = bExcludeOptimizeContentWithOverlay;
            if(!bKeepControlSize)
                ctrlHosting.Size = Geometry.ConvertSize(rectBounds.Size);
            ctrlHosting.Visible = false;
            m_ctrlHosting = ctrlHosting;

            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);

            // init bounds info
            PointF ptPinLocation =
                new PointF(rectBounds.X + rectBounds.Width / 2, rectBounds.Y + rectBounds.Height / 2);
            SizeF szPinOffset = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);

            CreateBoundsInfo(ptPinLocation, szPinOffset, rectBounds.Size);

            this.BoundsInfo.Unit = measureUnits;

            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlNode"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public ControlNode(ControlNode src)
            : base(src)
        {
            if (src.m_ctrlHosting != null)
            {
                Type type = src.m_ctrlHosting.GetType();

                m_ctrlHosting = (Control)Activator.CreateInstance(type);
                m_ctrlHCParent = src.HCParent;

                // clone given control using PropertyDescriptors
                object objPropValue;
                object curObjPropValue;
                PropertyDescriptorCollection collPD = TypeDescriptor.GetProperties(m_ctrlHosting);

                foreach (PropertyDescriptor pd in collPD)
                {
                    if (!pd.IsBrowsable) continue;

                    objPropValue = pd.GetValue(src.m_ctrlHosting);
                    curObjPropValue = pd.GetValue(m_ctrlHosting);

                    ICloneable cloneable = objPropValue as ICloneable;
                    IList list = objPropValue as IList;

                    if (cloneable != null)
                        objPropValue = cloneable.Clone();

                    try
                    {
                        // try to add item collection
                        if (list != null)
                        {
                            // get collection property
                            IList newList = (IList)curObjPropValue;

                            // iterate throw items
                            foreach (object var in list)
                            {
                                cloneable = var as ICloneable;

                                if (cloneable != null)
                                    newList.Add(cloneable.Clone());
                                else
                                    newList.Add(var);
                            }
                        }
                        else
                        {
                            // try to set single object
                            pd.SetValue(m_ctrlHosting, objPropValue);
                        }
                    }
                    catch (Exception)
                    { 
                    }
                }

                m_ctrlHosting.Location = Point.Empty;
                m_styleActivate = src.m_styleActivate;
                m_ctrlSnapshot = src.m_ctrlSnapshot;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlNode"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ControlNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Type type = null;
            Control ctrl = new Control();
            Hashtable hashProps = new Hashtable();

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "ctrlType":
                        type = (Type)info.GetValue("ctrlType", typeof(Type));
                        break;
                    case "activateStyle":
                        m_styleActivate = (ActivateStyle)info.GetValue("activateStyle", typeof(ActivateStyle));
                        break;
                    case "hcproperties":
                        m_hashHCP = (Hashtable)info.GetValue("hcproperties", typeof(Hashtable));
                        if (m_hashHCP != null)
                            m_hashHCP.OnDeserialization(this);
                        break;
                    case "propValues":
                        // clone given control using reflection
                        hashProps = (Hashtable)info.GetValue("propValues", typeof(Hashtable));
                        break;
                }
            }

            ctrl = (Control)Activator.CreateInstance(type);

            if (hashProps != null)
                hashProps.OnDeserialization(this);

            PropertyDescriptorCollection collPD = TypeDescriptor.GetProperties(ctrl);

            foreach (PropertyDescriptor pd in collPD)
            {
                if (!pd.IsBrowsable) continue;

                if (hashProps != null && hashProps.ContainsKey(pd.Name))
                    pd.SetValue(ctrl, hashProps[pd.Name]);
            }

            m_ctrlHosting = ctrl;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets a value indicating whether control size is used to print the control.
        /// </summary>
        public bool KeepControlSize
        {
            get { return m_bKeepControlSize; }
        }

        /// <summary>
        /// Gets a value indicating whether the rendering of control node is excluded from the background when OptimizedContentWithOverlay is true.
        /// <Remarks> The ExcludeOptimizeContentWithOverlay is only applicable for DiagramWebControl. </Remarks>
        /// </summary>
        public bool ExcludeOptimizeContentWithOverlay
        {
            get { return m_bExcludeOptimizeContentWithOverlay; }
        }

        /// <summary>
        /// Gets hashtable for custom controls properties.
        /// Used during serialization.
        /// </summary>
        protected Hashtable HCP
        {
            get
            {
                if (m_hashHCP == null)
                {
                    m_hashHCP = new Hashtable();
                }

                return m_hashHCP;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Node is activated.
        /// </summary>
        public bool Activated
        {
            get { return m_bActive; }
        }

        /// <summary>
        /// Gets or sets the host control parent.
        /// </summary>
        /// <value>The host control parent.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Control HCParent
        {
            get 
            { 
                return m_ctrlHCParent; 
            }
            set
            {
                if (m_ctrlHCParent != value)
                {
                    // assign new value
                    m_ctrlHCParent = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the hosting control.
        /// </summary>
        /// <value>The hosting control.</value>
        [Browsable(true)]
        [Description("The Windows Forms Control hosted by the node.")]
        public Control HostingControl
        {
            get 
            { 
                return m_ctrlHosting; 
            }
            set
            {
                if (m_ctrlHosting != value)
                    m_ctrlHosting = value;
            }
        }

        /// <summary>
        /// Gets the hosting control snapshot.
        /// </summary>
        /// <value>The snapshot.</value>
        [Browsable(true)]
        [Description("The snapshot of the hosting control.")]
        public Image Snapshot
        {
            get
            {
                return m_ctrlSnapshot;
            }
        }

        /// <summary>
        /// Gets or sets the activate style.
        /// </summary>
        /// <value>The activate style.</value>
        [Browsable(true)]
        [Description("The activation mode for the control.")]
        [DefaultValue(ActivateStyle.None)]
        public ActivateStyle ActivateStyle
        {
            get { return m_styleActivate; }
            set { m_styleActivate = value; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Activate ControlNode.
        /// </summary>
        /// <param name="ptHCLocation">Hosting control location in client coordinates.</param>
        public void Activate(PointF ptHCLocation)
        {
            if (this.HostingControl != null && OnPropertyChanging(this.FullContainerName, DPN.Activate, true))
            {
                // Assign Hositng Control new location
                this.HostingControl.Location = Geometry.ConvertPoint(ptHCLocation);

                // Activate Hosting control
                this.HostingControl.Visible = true;
                this.HostingControl.Parent = this.HCParent;

                m_bActive = true;

                OnPropertyChanged(this.FullContainerName, DPN.Activate);
            }
        }

        /// <summary>
        /// Deactivates control node.
        /// </summary>
        public void Deactivate()
        {
            if (this.HostingControl != null && this.OnPropertyChanging(this.FullContainerName, DPN.Activate, false))
            {
                UpdateControlSnapshot();
                // Activate Hosting control
                this.HostingControl.Visible = false;
                this.HostingControl.Parent = null;

                // Assign Hositng Control new location
                this.HostingControl.Location = Point.Empty;

                m_bActive = false;

                OnPropertyChanged(this.FullContainerName, DPN.Activate);
            }
        }

        /// <summary>
        /// Updates the HostingControl's snapshot.
        /// </summary>
        public void UpdateControlSnapshot()
        {
            Size szNodeSize;
            if (m_bKeepControlSize)
                szNodeSize = this.HostingControl.Size;
            else
                szNodeSize = Geometry.ConvertSize(this.BoundsInfo.GetSize(MeasureUnits.Pixel));
            m_ctrlSnapshot = ControlSnapshot.PrintControl(this.HostingControl, szNodeSize);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (this.HCP.Count > 0)
                info.AddValue("hcproperties", this.HCP);

            info.AddValue("activateStyle", m_styleActivate);
            info.AddValue("ctrlType", m_ctrlHosting.GetType());

            Hashtable hashProps = new Hashtable();

            // clone given control using reflection
            object objPropValue;
            PropertyDescriptorCollection collPD = TypeDescriptor.GetProperties(m_ctrlHosting);

            foreach (PropertyDescriptor pd in collPD)
            {
                if (!pd.IsBrowsable || !IsHCPropertySerializable(pd)) continue;

                objPropValue = pd.GetValue(m_ctrlHosting);

                if (objPropValue is Cursor) continue;

                if (objPropValue != null)
                {
                    if (objPropValue is ICloneable)
                    {
                        objPropValue = ((ICloneable)objPropValue).Clone();
                    }

                    hashProps.Add(pd.Name, objPropValue);
                }
            }

            info.AddValue("propValues", hashProps);
        }

        /// <summary>
        /// Serialize the node
        /// </summary>
        public virtual void SerializeNode()
        {
            if (this.m_localSerializedData == null)
                this.m_localSerializedData = new Hashtable();
            else
                this.m_localSerializedData.Clear();


            if (this.HCP.Count > 0)
                this.m_localSerializedData.Add("hcproperties", this.HCP);

            this.m_localSerializedData.Add("activateStyle", m_styleActivate);
            this.m_localSerializedData.Add("ctrlType", m_ctrlHosting.GetType());

            Hashtable hashProps = new Hashtable();

            // clone given control using reflection
            object objPropValue;
            PropertyDescriptorCollection collPD = TypeDescriptor.GetProperties(m_ctrlHosting);

            foreach (PropertyDescriptor pd in collPD)
            {
                if (!pd.IsBrowsable || !IsHCPropertySerializable(pd)) continue;

                objPropValue = pd.GetValue(m_ctrlHosting);

                if (objPropValue is Cursor) continue;

                if (objPropValue != null)
                {
                    if (objPropValue is ICloneable)
                    {
                        objPropValue = ((ICloneable)objPropValue).Clone();
                    }

                    hashProps.Add(pd.Name, objPropValue);
                }
            }

            this.m_localSerializedData.Add("propValues", hashProps);
        }

        /// <summary>
        /// Check whether the node has serialized data
        /// </summary>
        public bool HasSerializedData
        {
            get
            {
                if (this.m_localSerializedData != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Deserializes the node.
        /// </summary>
        public virtual void DeSerializeNode()
        {
            Type type = null;
            Control ctrl = new Control();
            Hashtable hashProps = new Hashtable();

            foreach (object entry in this.m_localSerializedData.Keys)
            {
                switch (entry.ToString())
                {
                    case "ctrlType":
                        type = (Type)this.m_localSerializedData["ctrlType"];
                        break;

                    case "hcproperties":
                        m_hashHCP = (Hashtable)this.m_localSerializedData["hcproperties"];
                        if (m_hashHCP != null)
                            m_hashHCP.OnDeserialization(this);
                        break;
                    case "propValues":
                        // clone given control using reflection
                        hashProps = (Hashtable)this.m_localSerializedData["propValues"];
                        break;
                }
            }

            ctrl = (Control)Activator.CreateInstance(type);

            if (hashProps != null)
                hashProps.OnDeserialization(this);

            PropertyDescriptorCollection collPD = TypeDescriptor.GetProperties(ctrl);

            foreach (PropertyDescriptor pd in collPD)
            {
                if (!pd.IsBrowsable) continue;

                if (hashProps != null && hashProps.ContainsKey(pd.Name))
                    pd.SetValue(ctrl, hashProps[pd.Name]);
            }

            m_ctrlHosting = ctrl;
        }

        /// <summary>
        /// Performs additional changes on size value changed.
        /// </summary>
        /// <param name="szOldSize">Old size value.</param>
        /// <param name="szNewSize">New size value.</param>
        protected override void DoSizeRelatedActions(SizeF szOldSize, SizeF szNewSize)
        {
            Size szNew = Geometry.ConvertSize(szNewSize);
            if (this.HostingControl.InvokeRequired)
            {
                DeSerializeNode();
            }
            if(!m_bKeepControlSize)
                this.HostingControl.SetBounds(0, 0, szNew.Width, szNew.Height, BoundsSpecified.All);
            base.DoSizeRelatedActions(szOldSize, szNewSize);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new ControlNode(this);
        }

        /// <summary>
        /// Renders shapes visual representation.
        /// on given graphics
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void Render(Graphics gfx)
        {
            base.Render(gfx);

            if (this.HostingControl != null)
            {
                if (!m_bActive)
                {
                    // this code is needed to prevent "HDC" exceptiont under Windows 2000
                    // move hosting control above client area
                    this.HostingControl.Location = new Point(int.MinValue, int.MinValue);
                    this.HostingControl.Visible = true;
                    //this.HostingControl.Visible = false;
                }
                RenderControlState(gfx);
            }
        }

         /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        public override void OnPropertyChanged(string strPropertyContainerName, string strPropertyName)
        {
            base.OnPropertyChanged(strPropertyContainerName, strPropertyName);
            if (strPropertyName == DPN.Size)
            {
                UpdateControlSnapshot();
            }
        }
        #endregion

        #region Class helper methods
        private bool IsHCPropertySerializable(PropertyDescriptor pd)
        {
            bool bSuccess = false;

            object objTmp = pd.GetValue(m_ctrlHosting);

            if (objTmp != null)
            {
                bSuccess = IsSerializable(objTmp);
            }

            return bSuccess;
        }
        private void RenderControlState(Graphics gfx)
        {
            Size szNodeSize;
            if (m_bKeepControlSize)
                szNodeSize = this.HostingControl.Size;
            else
                szNodeSize = Geometry.ConvertSize(this.BoundsInfo.GetSize(MeasureUnits.Pixel));
            Size controlSize = szNodeSize;
            if (this.Root != null)
            {
                float pageScale = this.Root.m_fMagnification / 100f;
                if (pageScale == gfx.PageScale && this.Root.m_fMagnification >= 100)
                {
                    controlSize = new Size((int)(szNodeSize.Width * gfx.PageScale), (int)(szNodeSize.Height * gfx.PageScale));
                }
            }

            // assign hosting control parent explicitly
            // FIXED bug with DataGrid rendering
            bool bHasParent = (this.HostingControl.Parent != null);

            // FIXED bug with Button control while set parent
            bool bButtonControl = this.HostingControl is Button;

            if (!bButtonControl && !bHasParent && !(this.HostingControl is System.Windows.Forms.Label))
                this.HostingControl.Parent = this.HCParent;

            if (m_ctrlSnapshot == null)
                m_ctrlSnapshot = ControlSnapshot.PrintControl(this.HostingControl, szNodeSize);
            // render metafile on to given graphics
            gfx.DrawImage(this.Snapshot, 0, 0, szNodeSize.Width, szNodeSize.Height);

            if (!this.Activated)
            {
                // hide hosting control
                this.HostingControl.Visible = false;
            }
        }
        private bool IsSerializable(object objPropValue)
        {
            TypeAttributes attr = objPropValue.GetType().Attributes;
            return (attr & TypeAttributes.Serializable) == TypeAttributes.Serializable;
        }
        #endregion
    }
}
