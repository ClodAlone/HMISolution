#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Encapsulates the edit properties of an object.
    /// </summary>
    [Serializable]
    public class EditStyle
        : PropertyContainer
    {
        #region Class internal declarations
        /// <summary>
        /// Flag enum used to store EditStyle
        /// </summary>
        [Flags]
        enum InternalEditStyle
        {
            /// <summary>
            /// Flag indicating whether node can be selected.
            /// </summary>
            ALLOW_SELECT = 1,

            /// <summary>
            /// Flag indicating whether node can be selected.
            /// </summary>
            ALLOW_DELETE = 2,

            /// <summary>
            /// Flag indicating whether node's vertexes can be edited.
            /// </summary>
            ALLOW_VERTEX_EDIT = 4,

            /// <summary>
            /// Flag indicating whether node can be moved by X axis.
            /// </summary>
            ALLOW_MOVE_BY_X_AXIS = 8,

            /// <summary>
            /// Flag indicating whether node can be moved by Y axis.
            /// </summary>
            ALLOW_MOVE_BY_Y_AXIS = 16,

            /// <summary>
            /// Flag indicating whether node can be rotated.
            /// </summary>
            ALLOW_ROTATE = 32,

            /// <summary>
            /// Flag indicating whether node's width can be changed.
            /// </summary>
            ALLOW_CHANGE_WIDTH = 64,

            /// <summary>
            /// Flag indicating whether node's height can be changed.
            /// </summary>
            ALLOW_CHANGE_HEIGHT = 128,

            /// <summary>
            /// Flag indicating whether node's height can be changed.
            /// </summary>
            ASPECT_RATIO_RESIZE = 256,

            /// <summary>
            /// Flag indicating whether node is enabled.
            /// </summary>
            ENABLED = 512,

            /// <summary>
            /// Flag indication whether rotation handle is visible.
            /// </summary>
            HIDE_ROTATION_HANDLE = 1024,

            /// <summary>
            /// Flag indication whether pin point is visible.
            /// </summary>
            HIDE_PIN_POINT = 2048
        }
        #endregion

        #region Constants
        private const string c_strSET_PROPERTY = "SetProperty";
        #endregion

        #region Class members
        private InternalEditStyle m_editStyle;
        private HandleEditMode m_editMode;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="EditStyle"/> class.
        /// </summary>
        public EditStyle()
        {
            DefaultInitialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditStyle"/> class.
        /// </summary>
        /// <param name="src">The edit style.</param>
        public EditStyle(EditStyle src)
            : base(src)
        {
            m_editStyle = src.m_editStyle;
            m_editMode = src.m_editMode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditStyle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected EditStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_editStyle = (InternalEditStyle)info.GetValue("editStyle", typeof(InternalEditStyle));
            m_editMode = (HandleEditMode)info.GetValue("editMode", typeof(HandleEditMode));
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether node can be resized.
        /// Property is obsolete.
        /// Use <see cref="AllowChangeHeight"/> and <see cref="AllowChangeWidth"/> properties instead.
        /// </summary>
        [Obsolete]
        [Browsable(false)]
        public bool AllowResize
        {
            get 
            { 
                return this.AllowChangeHeight && this.AllowChangeWidth; 
            }
            set
            {
                HistoryManager history = this.HistoryService;

                if (history != null)
                    history.StartAtomicAction(c_strSET_PROPERTY);

                this.AllowChangeWidth = value;
                this.AllowChangeHeight = value;

                if (history != null)
                    history.EndAtomicAction();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node can be moved.
        /// Property is obsolete.
        /// Use <see cref="AllowMoveX"/> and <see cref="AllowMoveY"/> properties instead.
        /// </summary>
        [Obsolete]
        [Browsable(false)]
        public bool AllowMove
        {
            get 
            { 
                return this.AllowMoveX && this.AllowMoveY; 
            }
            set
            {
                HistoryManager history = this.HistoryService;

                if (history != null)
                    history.StartAtomicAction(c_strSET_PROPERTY);

                this.AllowMoveX = value;
                this.AllowMoveY = value;

                if (history != null)
                    history.EndAtomicAction();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether object can be deleted.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether primitive can be deleted from its contaner.")]
        [DefaultValue(true)]
        public bool AllowDelete
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ALLOW_DELETE) == InternalEditStyle.ALLOW_DELETE; 
            }
            set
            {
                bool bOldValue = this.AllowDelete;

                if (bOldValue != value)
                    ChangeValue(DPN.AllowDelete, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object can be selected.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether primitive can be selected.")]
        [DefaultValue(true)]
        public bool AllowSelect
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ALLOW_SELECT) == InternalEditStyle.ALLOW_SELECT; 
            }
            set
            {
                bool bOldValue = this.AllowSelect;

                if (bOldValue != value)
                    ChangeValue(DPN.AllowSelect, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether object's vertices can be edited.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether vertexes can de edited.")]
        [DefaultValue(true)]
        public bool AllowVertexEdit
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ALLOW_VERTEX_EDIT) == InternalEditStyle.ALLOW_VERTEX_EDIT; 
            }
            set
            {
                bool bOldValue = this.AllowVertexEdit;

                if (bOldValue != value)
                {
                    if (!value && DefaultHandleEditMode == HandleEditMode.Vertex)
                    {
                        DefaultHandleEditMode = HandleEditMode.Resize;
                    }

                    ChangeValue(DPN.AllowVertexEdit, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object can be moved in X direction.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether moving along X axis is allowed.")]
        [DefaultValue(true)]
        public bool AllowMoveX
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ALLOW_MOVE_BY_X_AXIS) == InternalEditStyle.ALLOW_MOVE_BY_X_AXIS; 
            }
            set
            {
                bool bOldValue = this.AllowMoveX;

                if (bOldValue != value)
                    ChangeValue(DPN.AllowMoveX, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object can be moved in Y axis.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether moving along Y axis is allowed.")]
        [DefaultValue(true)]
        public bool AllowMoveY
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ALLOW_MOVE_BY_Y_AXIS) == InternalEditStyle.ALLOW_MOVE_BY_Y_AXIS; 
            }
            set
            {
                bool bOldValue = this.AllowMoveY;

                if (bOldValue != value)
                    ChangeValue(DPN.AllowMoveY, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object can be rotated.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether rotation is allowed.")]
        [DefaultValue(true)]
        public bool AllowRotate
        {
            get 
            {
                return (m_editStyle & InternalEditStyle.ALLOW_ROTATE) == InternalEditStyle.ALLOW_ROTATE; 
                }
            set
            {
                bool bOldValue = this.AllowRotate;

                if (bOldValue != value)
                    ChangeValue(DPN.AllowRotate, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object's Height can be changed.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether height changing is allowed.")]
        [DefaultValue(true)]
        public bool AllowChangeHeight
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ALLOW_CHANGE_HEIGHT) == InternalEditStyle.ALLOW_CHANGE_HEIGHT; 
            }
            set
            {
                bool bOldValue = this.AllowChangeHeight;

                if (bOldValue != value)
                    ChangeValue(DPN.AllowChangeHeight, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object's Width can be changed.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether width changing is allowed.")]
        [DefaultValue(true)]
        public bool AllowChangeWidth
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ALLOW_CHANGE_WIDTH) == InternalEditStyle.ALLOW_CHANGE_WIDTH; 
            }
            set
            {
                bool bOldValue = this.AllowChangeWidth;

                if (bOldValue != value)
                    ChangeValue(DPN.AllowChangeWidth, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node will be resized proportionally.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether width and height will be updated progressively.")]
        public bool AspectRatio
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ASPECT_RATIO_RESIZE) == InternalEditStyle.ASPECT_RATIO_RESIZE; 
            }
            set
            {
                bool bOldValue = this.AspectRatio;

                if (bOldValue != value)
                    ChangeValue(DPN.AspectRatio, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node will display rotation handle in diagram.
        /// </summary>
        /// <value><c>true</c> if need to hide rotation handle; otherwise, <c>false</c>.</value>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether rotation handle will draw on diagram canvas.")]
        public bool HideRotationHandle
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.HIDE_ROTATION_HANDLE) == InternalEditStyle.HIDE_ROTATION_HANDLE; 
            }
            set
            {
                bool bOldValue = this.HideRotationHandle;

                if (bOldValue != value)
                    ChangeValue(DPN.HideRotationHandle, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node will display pin point in diagram.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether pin point will draw on diagram canvas.")]
        public bool HidePinPoint
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.HIDE_PIN_POINT) == InternalEditStyle.HIDE_PIN_POINT; 
            }
            set
            {
                bool bOldValue = this.HidePinPoint;

                if (bOldValue != value)
                    ChangeValue(DPN.HidePinPoint, value);
            }
        }

        /// <summary>
        /// Gets or sets default mode for editing the object using selection handles.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(HandleEditMode.Resize)]
        [Description("Specifies handle edit mode.")]
        public HandleEditMode DefaultHandleEditMode
        {
            get 
            { 
                return m_editMode; 
            }
            set
            {
                if (m_editMode != value && OnPropertyChanging(DPN.DefaultHandleEditMode, value))
                {
                    if (!(value == HandleEditMode.Vertex && !AllowVertexEdit))
                    {
                        //// make history record
                        RecordPropertyChanged(DPN.DefaultHandleEditMode);
                        //// assign new value
                        m_editMode = value;
                        //// raise property changed event
                        OnPropertyChanged(DPN.DefaultHandleEditMode);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object is enabled or disabled.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Indicates whether object is enabled.")]
        public bool Enabled
        {
            get 
            { 
                return (m_editStyle & InternalEditStyle.ENABLED) == InternalEditStyle.ENABLED; 
            }
            set
            {
                bool bOldValue = this.Enabled;

                if (bOldValue != value)
                    ChangeValue(DPN.Enabled, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether current instance inherit container measure units.
        /// </summary>
        /// <value>
        /// <c>true</c> if current instance inherit container measure units; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool InheritContainerMeasureUnits
        {
            get { return base.InheritContainerMeasureUnits; }
            set { base.InheritContainerMeasureUnits = value; }
        }

        /// <summary>
        /// Gets or sets the measure unit.
        /// </summary>
        /// <value>The measure unit.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override MeasureUnits MeasureUnit
        {
            get { return base.MeasureUnit; }
            set { base.MeasureUnit = value; }
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// Determines if the given node is enabled or disabled.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>True if enabled; False if disabled.</returns>
        public static bool IsEnabled(Node node)
        {
            bool bResult = (node != null);

            if (bResult)
            {
                // get needed value
                bResult = node.EditStyle.Enabled;
            }

            return bResult;
        }

        /// <summary>
        /// Determines if the given node can be deleted from its container or not.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>True if node can be deleted; otherwise False.</returns>
        public static bool CanDelete(Node node)
        {
            bool bResult = (node != null);

            if (bResult)
            {
                bResult = node.EditStyle.AllowDelete;
            }

            return bResult;
        }

        /// <summary>
        /// Determines if the given node can be selected or not.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>True if node can be selected; otherwise False.</returns>
        public static bool CanSelect(Node node)
        {
            bool bResult = (node != null);

            if (bResult)
            {
                bResult = node.EditStyle.AllowSelect && node.EditStyle.Enabled;
            }

            return bResult;
        }

        /// <summary>
        /// Determines if the given node can be moved by X axis.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>True if node can be moved; otherwise False.</returns>
        public static bool CanMoveX(Node node)
        {
            bool bResult = (node != null);

            if (bResult)
            {
                // get needed values
                bResult = node.EditStyle.AllowMoveX && node.EditStyle.Enabled;
            }

            return bResult;
        }

        /// <summary>
        /// Determines if the given node can be moved or not.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>True if node can be moved; otherwise False.</returns>
        public static bool CanMoveY(Node node)
        {
            bool bResult = (node != null);

            if (bResult)
            {
                // get needed values
                bResult = node.EditStyle.AllowMoveY && node.EditStyle.Enabled;
            }

            return bResult;
        }

        /// <summary>
        /// Determines if the given node's width can be changed.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>True if node's Width can be changed; otherwise False.</returns>
        public static bool CanChangeWidth(Node node)
        {
            bool bResult = (node != null);

            if (bResult)
            {
                // get needed values
                bResult = node.EditStyle.Enabled && node.EditStyle.AllowChangeWidth;

                if (bResult && !node.EditStyle.AllowChangeHeight && node.EditStyle.AspectRatio)
                {
                    bResult = false;
                }
            }

            return bResult;
        }

        /// <summary>
        /// Determines if the given node's width can be changed.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>True if node's Width can be changed; otherwise False.</returns>
        public static bool CanChangeHeight(Node node)
        {
            bool bResult = (node != null);

            if (bResult)
            {
                // get needed values
                bResult = node.EditStyle.Enabled && node.EditStyle.AllowChangeHeight;

                if (bResult && !node.EditStyle.AllowChangeWidth && node.EditStyle.AspectRatio)
                {
                    bResult = false;
                }
            }

            return bResult;
        }

        /// <summary>
        /// Determines whether given node can be rotated.
        /// </summary>
        /// <param name="node">Node to test</param>
        /// <returns>True if node can be moved; otherwise False.</returns>
        public static bool CanRotate(Node node)
        {
            bool bResult = (node != null);

            if (bResult)
            {
                // get needed values
                bResult = node.EditStyle.AllowRotate && node.EditStyle.Enabled;
            }

            return bResult;
        }

        // public static bool IsNodeEnabled(INode node)
        // {
        //    bool isEnabled = false;
        //
        //    if (node != null)
        //    {
        //       EditStyle editStyle = node.GetService(typeof(EditStyle)) as EditStyle;
        //       if (editStyle != null)
        //       {
        //           isEnabled = editStyle.Enabled;
        //       }
        //       else
        //       {
        //           isEnabled = true;
        //       }
        //   }
        //
        //   return isEnabled;
        // }
        //
        // /// <summary>
        // /// Determines if the given node can be selected or not.
        // /// </summary>
        // /// <param name="node">Node to test.</param>
        // /// <returns>True if node can be selected; otherwise False.</returns>
        // public static bool CanSelect(INode node)
        // {
        //   bool allowSelect = false;
        //   bool isEnabled = false;
        //   bool bvisible = true;
        //
        //   if (node != null)
        //   {
        //       EditStyle editStyle = node.GetService(typeof(EditStyle)) as EditStyle;
        //       if (editStyle != null)
        //       {
        //          allowSelect = editStyle.AllowSelect;
        //          isEnabled = editStyle.Enabled;
        //       }
        //       else
        //       {
        //          allowSelect = true;
        //          isEnabled = true;
        //       }
        //
        //       IPropertyContainer propcontainer = node.GetService(typeof(IPropertyContainer)) as IPropertyContainer;
        //       if(propcontainer != null)
        //       {
        //           object visible = propcontainer.GetPropertyValue("Visible");
        //           if(visible != null)
        //               bvisible = (bool)visible;
        //       }
        //
        //    }
        //
        //    return (allowSelect && isEnabled && bvisible);
        // }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Defaults values in initialization.
        /// </summary>
        private void DefaultInitialize()
        {
            m_editStyle = InternalEditStyle.ALLOW_CHANGE_HEIGHT | InternalEditStyle.ALLOW_CHANGE_WIDTH
                          | InternalEditStyle.ALLOW_MOVE_BY_X_AXIS | InternalEditStyle.ALLOW_MOVE_BY_Y_AXIS
                          | InternalEditStyle.ALLOW_ROTATE | InternalEditStyle.ALLOW_SELECT | InternalEditStyle.ALLOW_DELETE
                          | InternalEditStyle.ALLOW_VERTEX_EDIT
                          | InternalEditStyle.ENABLED;

            m_editMode = HandleEditMode.Resize;
        }

        /// <summary>
        /// Changes the value.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        private void ChangeValue(string strPropertyName, object newValue)
        {
            if (OnPropertyChanging(strPropertyName, newValue))
            {
                //// make history entry
                RecordPropertyChanged(strPropertyName);
                //// assign new value
                AssignNewValue(strPropertyName, (bool)newValue);
                //// raise property changed event
                OnPropertyChanged(strPropertyName);
            }
        }

        /// <summary>
        /// Assigns the new value.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">new value, if set to <c>true</c>.</param>
        private void AssignNewValue(string strPropertyName, bool newValue)
        {
            switch (strPropertyName)
            {
                case "AllowSelect":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ALLOW_SELECT;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ALLOW_SELECT);
                    break;
                case "AllowDelete":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ALLOW_DELETE;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ALLOW_DELETE);
                    break;
                case "AllowMoveX":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ALLOW_MOVE_BY_X_AXIS;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ALLOW_MOVE_BY_X_AXIS);
                    break;
                case "AllowMoveY":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ALLOW_MOVE_BY_Y_AXIS;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ALLOW_MOVE_BY_Y_AXIS);
                    break;
                case "AllowVertexEdit":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ALLOW_VERTEX_EDIT;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ALLOW_VERTEX_EDIT);
                    break;
                case "AllowRotate":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ALLOW_ROTATE;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ALLOW_ROTATE);
                    break;
                case "AllowChangeHeight":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ALLOW_CHANGE_HEIGHT;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ALLOW_CHANGE_HEIGHT);
                    break;
                case "AllowChangeWidth":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ALLOW_CHANGE_WIDTH;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ALLOW_CHANGE_WIDTH);
                    break;
                case "AspectRatio":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ASPECT_RATIO_RESIZE;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ASPECT_RATIO_RESIZE);
                    break;
                case "Enabled":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.ENABLED;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.ENABLED);
                    break;
                case "HideRotationHandle":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.HIDE_ROTATION_HANDLE;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.HIDE_ROTATION_HANDLE);
                    break;
                case "HidePinPoint":
                    if (newValue)
                        m_editStyle |= InternalEditStyle.HIDE_PIN_POINT;
                    else
                        m_editStyle = m_editStyle & (~InternalEditStyle.HIDE_PIN_POINT);
                    break;
            }
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.EditStyle;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new EditStyle(this);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("editStyle", m_editStyle);
            info.AddValue("editMode", m_editMode);
        }
        #endregion
    }
}
