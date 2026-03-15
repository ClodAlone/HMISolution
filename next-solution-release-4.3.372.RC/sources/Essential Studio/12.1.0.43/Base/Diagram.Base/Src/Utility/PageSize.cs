#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The size type object with specified measurement units.
    /// </summary>
    [Serializable]
    public class MeasureSize
        : ICloneable,
          IServiceReferenceHolder,
          IPropertyContainer,
          IPropertyObserver,
          ISerializable
    {
        #region Class members
        private EventSink m_eventSink;
        private IServiceReferenceProvider m_provider;
        private HistoryManager m_mgrHistoryManager;
        protected IPropertyObserver m_propertyObserver;
        private IPropertyContainer m_owner;

        private string m_strDisplayName;
        private float m_fPixelWidth;
        private float m_fPixelHeight;
        private MeasureUnits m_widthUnit;
        private MeasureUnits m_heightUnit;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [Browsable(false)]
        [DefaultValue("Custom")]
        public string DisplayName
        {
            get { return m_strDisplayName; }
            set { m_strDisplayName = value; }
        }

        /// <summary>
        /// Gets the width of the pixel.
        /// </summary>
        /// <value>The width of the pixel.</value>
        [Browsable(false)]
        public float PixelWidth
        {
            get { return m_fPixelWidth; }
        }

        /// <summary>
        /// Gets the height of the pixel.
        /// </summary>
        /// <value>The height of the pixel.</value>
        [Browsable(false)]
        public float PixelHeight
        {
            get { return m_fPixelHeight; }
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>The owner.</value>
        [Browsable(false)]
        public View Owner
        {
            get { return m_owner as View; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        protected float Width
        {
            get 
            { 
                return MeasureUnitsConverter.FromPixelX(this.PixelWidth, this.WidthMeasureUnit); 
            }
            set
            {
                float fWidth = this.Width;

                if (fWidth != value && OnPropertyChanging(DPN.Width, fWidth))
                {
                    RecordPropertyChanged(DPN.Width);

                    // update pixel size
                    m_fPixelWidth = MeasureUnitsConverter.ToPixelX(value, this.WidthMeasureUnit);

                    OnPropertyChanged(DPN.Width);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        protected float Height
        {
            get 
            { 
                return MeasureUnitsConverter.FromPixelX(this.PixelHeight, this.HeightMeasureUnit); 
            }
            set
            {
                float fHeight = this.Height;

                if (fHeight != value && OnPropertyChanging(DPN.Height, fHeight))
                {
                    RecordPropertyChanged(DPN.Height);

                    // update pixel size
                    m_fPixelHeight = MeasureUnitsConverter.ToPixelY(value, this.HeightMeasureUnit);

                    OnPropertyChanged(DPN.Height);
                }
            }
        }

        /// <summary>
        /// Gets or sets the width measure unit.
        /// </summary>
        /// <value>The width measure unit.</value>
        protected MeasureUnits WidthMeasureUnit
        {
            get 
            { 
                return m_widthUnit; 
            }
            set
            {
                if (m_widthUnit != value && OnPropertyChanging(DPN.WidthUnit, m_widthUnit))
                {
                    RecordPropertyChanged(DPN.WidthUnit);

                    // set new value
                    m_widthUnit = value;

                    OnPropertyChanged(DPN.WidthUnit);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height measure unit.
        /// </summary>
        /// <value>The height measure unit.</value>
        protected MeasureUnits HeightMeasureUnit
        {
            get 
            { 
                return m_heightUnit; 
            }
            set
            {
                if (m_heightUnit != value && OnPropertyChanging(DPN.HeightUnit, m_heightUnit))
                {
                    RecordPropertyChanged(DPN.HeightUnit);

                    // set new value
                    m_heightUnit = value;

                    OnPropertyChanged(DPN.HeightUnit);
                }
            }
        }

        /// <summary>
        /// Gets the history service.
        /// </summary>
        /// <value>The history service.</value>
        protected HistoryManager HistoryService
        {
            get { return m_mgrHistoryManager; }
        }

        /// <summary>
        /// Gets the property observer.
        /// </summary>
        /// <value>The property observer.</value>
        protected IPropertyObserver PropertyObserber
        {
            get
            {
                return m_propertyObserver;
            }
        }
        #endregion

        #region Class initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureSize"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public MeasureSize(MeasureSize src)
        {
            m_strDisplayName = src.m_strDisplayName;
            m_widthUnit = src.m_widthUnit;
            m_heightUnit = src.m_heightUnit;

            m_fPixelWidth = src.m_fPixelWidth;
            m_fPixelHeight = src.m_fPixelHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureSize"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="width">The width.</param>
        /// <param name="widthUnit">The width unit.</param>
        /// <param name="height">The height.</param>
        /// <param name="heightUnit">The height unit.</param>
        public MeasureSize(string name, float width, MeasureUnits widthUnit, float height, MeasureUnits heightUnit)
        {
            m_strDisplayName = name;

            m_fPixelWidth = MeasureUnitsConverter.ToPixelX(width, widthUnit);
            m_fPixelHeight = MeasureUnitsConverter.ToPixelY(height, heightUnit);

            m_widthUnit = widthUnit;
            m_heightUnit = heightUnit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureSize"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="widthUnit">The width unit.</param>
        /// <param name="height">The height.</param>
        /// <param name="heightUnit">The height unit.</param>
        public MeasureSize(float width, MeasureUnits widthUnit, float height, MeasureUnits heightUnit)
            : this("Custom", width, widthUnit, height, heightUnit)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureSize"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public MeasureSize(string name, float width, float height)
            : this(name, width, MeasureUnits.Pixel, height, MeasureUnits.Pixel)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureSize"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public MeasureSize(float width, float height)
            : this(width, MeasureUnits.Pixel, height, MeasureUnits.Pixel)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureSize"/> class.
        /// </summary>
        public MeasureSize()
            : this(0f, 0f)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureSize"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public MeasureSize(SerializationInfo info, StreamingContext context)
        {
            this.DisplayName = info.GetString("displayName");
            this.WidthMeasureUnit = (MeasureUnits)info.GetValue("widthMeasureUnit", typeof(MeasureUnits));
            this.HeightMeasureUnit = (MeasureUnits)info.GetValue("heightMeasureUnit", typeof(MeasureUnits));

            m_fPixelWidth = info.GetSingle("pixelWidth");
            m_fPixelHeight = info.GetSingle("pixelHeight");
        }
        #endregion

        #region Class override methods
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <returns>The size.</returns>
        public SizeF GetSize(MeasureUnits units)
        {
            float fWidth = MeasureUnitsConverter.FromPixelX(this.PixelWidth, units);
            float fHeight = MeasureUnitsConverter.FromPixelY(this.PixelHeight, units);

            return new SizeF(fWidth, fHeight);
        }

        /// <summary>
        /// Sets the size.
        /// </summary>
        /// <param name="fWidth">Width of the f.</param>
        /// <param name="fHeight">Height of the f.</param>
        /// <param name="measureUnits">The measure units.</param>
        public void SetSize(float fWidth, float fHeight, MeasureUnits measureUnits)
        {
            m_widthUnit = measureUnits;
            m_heightUnit = measureUnits;
            this.Width = fWidth;
            this.Height = fHeight;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Records the property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the STR property.</param>
        protected void RecordPropertyChanged(string strPropertyName)
        {
            if (this.HistoryService != null)
            {
                this.HistoryService.RecordPropertyChanged(this, string.Empty, strPropertyName);
            }
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            return new MeasureSize(this);
        }
        #endregion

        #region IServiceReferenceHolder Members
        /// <summary>
        /// Updates the service references from service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public virtual void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            if (provider == null)
            {
                m_mgrHistoryManager = null;
                m_propertyObserver = null;
                m_owner = null;
                m_eventSink = null;
            }
            else
            {
                m_mgrHistoryManager = (HistoryManager)provider.ProvideServiceReference(typeof(HistoryManager).TypeHandle);
                m_propertyObserver = (IPropertyObserver)provider.ProvideServiceReference(typeof(IPropertyObserver).TypeHandle);
                m_owner = (IPropertyContainer)provider.ProvideServiceReference(typeof(IPropertyContainer).TypeHandle);
                m_eventSink = (EventSink)provider.ProvideServiceReference(typeof(EventSink).TypeHandle);
            }
            if (provider is Model)
                m_provider = provider;
        }
        #endregion

        #region IPropertyObserver Members
        /// <summary>
        /// Called when property is changing.
        /// </summary>
        /// <param name="strPropertyFullPath">The full path.</param>
        /// <param name="strPropertyName">The Name.</param>
        /// <param name="oldValue">The old value.</param>
        /// <returns>true, if property changing.</returns>
        public bool OnPropertyChanging(string strPropertyFullPath, string strPropertyName, object oldValue)
        {
            return OnPropertyChanging(strPropertyName, oldValue);
        }

        /// <summary>
        /// Called when property is changed.
        /// </summary>
        /// <param name="strPropertyFullPath">The full path.</param>
        /// <param name="strPropertyName">The Name.</param>
        public void OnPropertyChanged(string strPropertyFullPath, string strPropertyName)
        {
            OnPropertyChanged(strPropertyName);
        }

        /// <summary>
        /// Called when property is changing.
        /// </summary>
        /// <param name="strPropertyName">The property name.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>true, if property changing.</returns>
        protected virtual bool OnPropertyChanging(string strPropertyName, object newValue)
        {
            bool bSuccess = true;

            if (m_propertyObserver != null)
            {
                bSuccess = m_propertyObserver.OnPropertyChanging(this.FullContainerName, strPropertyName, newValue);
            }
            else if (m_provider != null)
            {
                m_propertyObserver = (IPropertyObserver)m_provider.ProvideServiceReference(typeof(IPropertyObserver).TypeHandle);
                if (m_propertyObserver != null)
                    bSuccess = m_propertyObserver.OnPropertyChanging(this.FullContainerName, strPropertyName, newValue);
            }
            return bSuccess;
        }

        /// <summary>
        /// Called when property is changed.
        /// </summary>
        /// <param name="strPropertyName">Property Name.</param>
        protected virtual void OnPropertyChanged(string strPropertyName)
        {
            if (m_propertyObserver != null)
            {
                m_propertyObserver.OnPropertyChanged(this.FullContainerName, strPropertyName);
            }
            else if (m_provider != null)
            {
                m_propertyObserver = (IPropertyObserver)m_provider.ProvideServiceReference(typeof(IPropertyObserver).TypeHandle);
                if (m_propertyObserver != null)
                    m_propertyObserver.OnPropertyChanged(this.FullContainerName, strPropertyName);
            }
        }
        #endregion

        #region IPropertyContainer Members
        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <returns>The object.</returns>
        public virtual object GetPropertyContainerByName(string strPropertyName)
        {
            return DPN.NodeScale;
        }

        /// <summary>
        /// Gets the full name of the container.
        /// </summary>
        /// <value>The full name of the container.</value>
        [Browsable(false)]
        public virtual string FullContainerName
        {
            get { return DPN.NodeScale; }
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("displayName", this.DisplayName);
            info.AddValue("pixelWidth", this.PixelWidth);
            info.AddValue("pixelHeight", this.PixelHeight);
            info.AddValue("widthMeasureUnit", this.WidthMeasureUnit);
            info.AddValue("heightMeasureUnit", this.HeightMeasureUnit);
        }
        #endregion
    }

    /// <summary>
    /// Use for configure virtual document size without any scaling.
    /// </summary>
    [Serializable]
    [Editor(typeof(PageSizeEditor), typeof(UITypeEditor))]
    public class PageSize : MeasureSize
    {
        #region Class properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [DefaultValue(0)]
        public new float Width
        {
            get { return base.Width; }
            set { base.Width = value; }
        }

        /// <summary>
        /// Gets or sets the width measure unit.
        /// </summary>
        /// <value>The width measure unit.</value>
        [DefaultValue(MeasureUnits.Pixel)]
        public new MeasureUnits WidthMeasureUnit
        {
            get { return base.WidthMeasureUnit; }
            set { base.WidthMeasureUnit = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DefaultValue(0)]
        public new float Height
        {
            get { return base.Height; }
            set { base.Height = value; }
        }

        /// <summary>
        /// Gets or sets the height measure unit.
        /// </summary>
        /// <value>The height measure unit.</value>
        [DefaultValue(MeasureUnits.Pixel)]
        public new MeasureUnits HeightMeasureUnit
        {
            get { return base.HeightMeasureUnit; }
            set { base.HeightMeasureUnit = value; }
        }
        #endregion

        #region Class initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PageSize"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public PageSize(PageSize src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSize"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="width">The width.</param>
        /// <param name="widthUnit">The width unit.</param>
        /// <param name="height">The height.</param>
        /// <param name="heightUnit">The height unit.</param>
        public PageSize(string name, float width, MeasureUnits widthUnit, float height, MeasureUnits heightUnit)
            : base(name, width, widthUnit, height, heightUnit)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSize"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="widthUnit">The width unit.</param>
        /// <param name="height">The height.</param>
        /// <param name="heightUnit">The height unit.</param>
        public PageSize(float width, MeasureUnits widthUnit, float height, MeasureUnits heightUnit)
            : this("Custom", width, widthUnit, height, heightUnit)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSize"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PageSize(string name, float width, float height)
            : this(name, width, MeasureUnits.Pixel, height, MeasureUnits.Pixel)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSize"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PageSize(float width, float height)
            : this(width, MeasureUnits.Pixel, height, MeasureUnits.Pixel)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSize"/> class.
        /// </summary>
        public PageSize()
            : this(0f, 0f)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSize"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public PageSize(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
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
            return new PageSize(this);
        }
        #endregion
    }

    /// <summary>
    /// Use for configure model and nodes measure scale.
    /// </summary>
    [Serializable]
    [Editor(typeof(PageScaleEditor), typeof(UITypeEditor))]
    public class PageScale : MeasureSize
    {
        #region Class initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PageScale"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public PageScale(PageScale src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageScale"/> class.
        /// </summary>
        public PageScale()
            : this(1f, 1f)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageScale"/> class.
        /// </summary>
        /// <param name="pageScale">The page scale.</param>
        /// <param name="drawingScale">The drawing scale.</param>
        public PageScale(float pageScale, float drawingScale)
            : base("No Scale", pageScale, drawingScale)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageScale"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pageScale">The page scale.</param>
        /// <param name="drawingScale">The drawing scale.</param>
        public PageScale(string name, float pageScale, float drawingScale)
            : base(name, pageScale, drawingScale)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageScale"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pageScale">The page scale.</param>
        /// <param name="pageUnit">The page unit.</param>
        /// <param name="drawingScale">The drawing scale.</param>
        /// <param name="drawingUnit">The drawing unit.</param>
        public PageScale(string name, float pageScale, MeasureUnits pageUnit, float drawingScale, MeasureUnits drawingUnit)
            : base(name, pageScale, pageUnit, drawingScale, drawingUnit)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageScale"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public PageScale(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [DefaultValue(0)]
        public new float Width
        {
            get { return (float)Math.Min(CommonUsedValues.MAX_SCALE_VALUE, Math.Max(CommonUsedValues.MIN_SCALE_VALUE, base.Width)); }
            set { base.Width = (float)Math.Min(CommonUsedValues.MAX_SCALE_VALUE, Math.Max(CommonUsedValues.MIN_SCALE_VALUE, value)); }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DefaultValue(0)]
        public new float Height
        {
            get { return (float)Math.Min(CommonUsedValues.MAX_SCALE_VALUE, Math.Max(CommonUsedValues.MIN_SCALE_VALUE, base.Height)); }
            set { base.Height = (float)Math.Min(CommonUsedValues.MAX_SCALE_VALUE, Math.Max(CommonUsedValues.MIN_SCALE_VALUE, value)); }
        }

        /// <summary>
        /// Gets or sets the model scale.
        /// </summary>
        /// <value>The model scale.</value>
        [DefaultValue(1f)]
        public float ModelScale
        {
            get { return this.Width; }
            set { this.Width = value; }
        }

        /// <summary>
        /// Gets or sets the model scale unit.
        /// </summary>
        /// <value>The model scale unit.</value>
        [DefaultValue(MeasureUnits.Pixel)]
        public MeasureUnits ModelScaleUnit
        {
            get { return base.WidthMeasureUnit; }
            set { base.WidthMeasureUnit = value; }
        }

        /// <summary>
        /// Gets or sets the drawing scale.
        /// </summary>
        /// <value>The drawing scale.</value>
        [DefaultValue(1f)]
        public float DrawingScale
        {
            get { return this.Height; }
            set { this.Height = value; }
        }

        /// <summary>
        /// Gets or sets the drawing scale unit.
        /// </summary>
        /// <value>The drawing scale unit.</value>
        [DefaultValue(MeasureUnits.Pixel)]
        public MeasureUnits DrawingScaleUnit
        {
            get { return base.HeightMeasureUnit; }
            set { base.HeightMeasureUnit = value; }
        }
        #endregion

        #region Class overrrides
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new PageScale(this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool bEqual = base.Equals(obj);
            PageScale pageScale = obj as PageScale;

            if (pageScale != null)
            {
                bEqual = (pageScale.Width == this.Width && pageScale.WidthMeasureUnit == this.WidthMeasureUnit
                    && pageScale.Height == this.Height && this.HeightMeasureUnit == this.HeightMeasureUnit
                    && pageScale.DisplayName == this.DisplayName);
            }

            return bEqual;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Gets the scale factor.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <returns>The scale factor.</returns>
        public float GetScaleFactor(MeasureUnits units)
        {
            float fFrom = this.PixelWidth;
            float fTo = this.PixelHeight;

            if (units != MeasureUnits.Pixel)
            {
                fFrom = MeasureUnitsConverter.FromPixelX(fFrom, units);
                fTo = MeasureUnitsConverter.FromPixelY(fTo, units);
            }

            return fFrom / fTo;
        }

        /// <summary>
        /// Gets the scale transformation.
        /// </summary>
        /// <param name="measureUnits">The measure units.</param>
        /// <returns>The matrix.</returns>
        public Matrix GetScaleTransformation(MeasureUnits measureUnits)
        {
            float fFactor = GetScaleFactor(measureUnits);
            return new Matrix(fFactor, 0, 0, fFactor, 0, 0);
        }
        #endregion
    }

    #region Type Editors
    /// <summary>
    /// Editor for modifying page size.
    /// </summary>
    public class PageSizeEditor : System.Drawing.Design.UITypeEditor
    {
        private IWindowsFormsEditorService edSvc = null;

        /// <summary>
        /// Sets the editor properties.
        /// </summary>
        /// <param name="editingInstance">The editing instance.</param>
        /// <param name="model">The model.</param>
        /// <param name="editor">The editor.</param>
        protected virtual void SetEditorProps(PageSize editingInstance, Model model, PageSizeDialog editor)
        {
            View view = editingInstance.Owner;

            if (view != null)
            {
                editor.PrinterSettings = (PageSettings)view.PageSettings.Clone();
            }

            if (model != null)
            {
                RectangleF rcRect = new HandleRenderer().GetBoundingRect(model.Nodes);
                float fWidth = Math.Max(rcRect.Right, model.MinimumSize.Width);
                float fHeight = Math.Max(rcRect.Bottom, model.MinimumSize.Height);

                editor.ModelContentSize = new SizeF(fWidth, fHeight);
            }

            editor.PageSize = (PageSize)editingInstance.Clone();
        }

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null
                && context.Instance != null
                && provider != null)
            {
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    if (context.Instance is Array)
                    {
                        Array ar = context.Instance as Array;
                        if (ar.Length > 0)
                        {
                            PageSize[] fsArray = new PageSize[ar.Length];
                            int i = 0;
                            foreach (object o in ar)
                            {
                                PageSize fs = null;

                                // get exact property name
                                // -----------------------
                                // FillStyle is used for defining BackgroundStyle also
                               
                                fs = (PageSize)TypeDescriptor.GetProperties(o, false)[context.PropertyDescriptor.Name].GetValue(o);
                                if (fs != null)
                                {
                                    fsArray[i] = fs;
                                    i++;
                                }
                            }
                            if (fsArray.Length > 0)
                            {
                                PageSizeDialog fsdlg = new PageSizeDialog();
                                SetEditorProps(fsArray[0], null, fsdlg);
                                if (DialogResult.OK == edSvc.ShowDialog(fsdlg))
                                {
                                    foreach (PageSize fs in fsArray)
                                    {
                                        value = UpdateReturnValue(fs, fsdlg);
                                    }
                                    foreach (object o in ar)
                                    {
                                        TypeDescriptor.GetProperties(o, false)[context.PropertyDescriptor.Name].SetValue(o, value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // get exact property name
                        // -----------------------
                        // FillStyle is used for defining BackgroundStyle also
                        Model model = context.Instance as Model;

                        PageSize fs = (PageSize)TypeDescriptor.GetProperties(
                            context.Instance, false)[context.PropertyDescriptor.Name].GetValue(context.Instance);

                        PageSizeDialog fsdlg = new PageSizeDialog();
                        SetEditorProps(fs, model, fsdlg);

                        if (DialogResult.OK == edSvc.ShowDialog(fsdlg))
                        {
                            value = UpdateReturnValue(fs, fsdlg);
                            TypeDescriptor.GetProperties(context.Instance)[context.PropertyDescriptor.Name].SetValue(context.Instance, value);
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Updates the return value.
        /// </summary>
        /// <param name="editingInstance">The editing instance.</param>
        /// <param name="editor">The editor.</param>
        /// <returns>The object.</returns>
        private static object UpdateReturnValue(PageSize editingInstance, PageSizeDialog editor)
        {
            View view = editingInstance.Owner;

            if (view != null)
            {
                view.PageSettings = editor.PrinterSettings;
            }

            return editor.PageSize;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
    }

    /// <summary>
    /// Editor for modifying page scale.
    /// </summary>
    public class PageScaleEditor : System.Drawing.Design.UITypeEditor
    {
        private IWindowsFormsEditorService edSvc = null;

        /// <summary>
        /// Sets the editor properties.
        /// </summary>
        /// <param name="editingInstance">The editing instance.</param>
        /// <param name="model">The model.</param>
        /// <param name="editor">The editor.</param>
        protected virtual void SetEditorProps(PageScale editingInstance, Model model, PageScaleDialog editor)
        {
            if (model != null)
            {
                editor.PageSize = model.DocumentSize;
                editor.MeasureUnits = model.MeasurementUnits;
            }

            editor.PageScale = editingInstance;
        }

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null
                && context.Instance != null
                && provider != null)
            {
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    if (context.Instance is Array)
                    {
                        Array ar = context.Instance as Array;
                        if (ar.Length > 0)
                        {
                            PageScale[] fsArray = new PageScale[ar.Length];
                            int i = 0;
                            foreach (object o in ar)
                            {
                                PageScale fs = null;

                                // get exact property name
                                // -----------------------
                                // FillStyle is used for defining BackgroundStyle also
                              
                                fs = (PageScale)TypeDescriptor.GetProperties(o, false)[context.PropertyDescriptor.Name].GetValue(o);
                                if (fs != null)
                                {
                                    fsArray[i] = fs;
                                    i++;
                                }
                            }
                            if (fsArray.Length > 0)
                            {
                                PageScaleDialog fsdlg = new PageScaleDialog();
                                SetEditorProps(fsArray[0], null, fsdlg);
                                if (DialogResult.OK == edSvc.ShowDialog(fsdlg))
                                {
                                    foreach (PageScale fs in fsArray)
                                    {
                                        value = UpdateReturnValue(null, fsdlg);
                                    }
                                    foreach (object o in ar)
                                    {
                                        TypeDescriptor.GetProperties(o, false)[context.PropertyDescriptor.Name].SetValue(o, value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // get exact property name
                        // -----------------------
                        // FillStyle is used for defining BackgroundStyle also
                        
                        Model model = context.Instance as Model;
                        Node node = context.Instance as Node;

                        if (model == null && node != null)
                            model = node.Root;

                        PageScale fs = (PageScale)TypeDescriptor.GetProperties(
                            context.Instance, false)[context.PropertyDescriptor.Name].GetValue(context.Instance);

                        PageScaleDialog fsdlg = new PageScaleDialog();
                        SetEditorProps(fs, model, fsdlg);

                        if (DialogResult.OK == edSvc.ShowDialog(fsdlg))
                        {
                            value = UpdateReturnValue(model, fsdlg);
                            TypeDescriptor.GetProperties(context.Instance)[context.PropertyDescriptor.Name].SetValue(context.Instance, value);
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Updates the return value.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="editor">The editor.</param>
        /// <returns>The object.</returns>
        private static object UpdateReturnValue(Model model, PageScaleDialog editor)
        {
            if (model != null)
            {
                model.MeasurementUnits = editor.MeasureUnits;
                model.DocumentSize = editor.PageSize;
            }

            return editor.PageScale;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
    }
    #endregion
}
