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
using System.Drawing;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A BoundsInfo is a collection of properties that define a Node transformation in current measure units.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="Syncfusion.Windows.Forms.Diagram.Node"/> to save pin point position, pin point offset, size, scale and measure units.
    /// </remarks>
    [Serializable]
    public sealed class BoundsInfo
        : PropertyContainer
    {
        #region Class members
        private DocumentEventSink m_eventSink;
        private float m_fHitTestPadding;
        private PointF m_ptPinPoint;
        private SizeF m_szPinOffset;
        private SizeF m_szSize;
        private MeasureUnits m_unitMeasure;
        private PageScale m_scale;
        private float m_fUnitDependentHitTestPadding;
        private PointF m_ptUnitDependentPinValue;
        private SizeF m_szUnitDependentPinOffsetValue;
        private SizeF m_szUnitDependentSizeValue;
        private MoveCallback m_callbackMove;
        private SizeCallback m_callbackSize;
        private PinOffsetCallback m_callbackPinOffset;
        private bool m_bIsResizing;
        private bool m_bIsSegmentChanging;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundsInfo"/> class from source instance.
        /// </summary>
        /// <param name="src">The source instance.</param>
        /// <param name="pinMoveCallback">The pin point move callback.</param>
        /// <param name="pinOffsetCallback">The pin point offset callback.</param>
        /// <param name="szSizeCallback">The size callback.</param>
        /// <param name="nodeScale">The node scale value.</param>
        internal BoundsInfo(BoundsInfo src, MoveCallback pinMoveCallback, PinOffsetCallback pinOffsetCallback, SizeCallback szSizeCallback, PageScale nodeScale)
            : this(src.m_ptPinPoint, pinMoveCallback, src.m_szPinOffset, pinOffsetCallback, src.m_szSize, szSizeCallback)
        {
            m_scale = nodeScale;

            m_unitMeasure = src.m_unitMeasure;

            // Update unit dependent values
            m_fUnitDependentHitTestPadding = MeasureUnitsConverter.FromPixelX(m_fHitTestPadding, m_unitMeasure);
            m_ptUnitDependentPinValue = MeasureUnitsConverter.FromPixels(m_ptPinPoint, m_unitMeasure);
            m_szUnitDependentSizeValue = MeasureUnitsConverter.FromPixels(m_szSize, m_unitMeasure);
            m_szUnitDependentPinOffsetValue = MeasureUnitsConverter.FromPixels(m_szPinOffset, m_unitMeasure);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundsInfo"/> class from 
        /// known pin point position, size and pin point offset values.
        /// </summary>
        /// <param name="ptPinPoint">The pin point position.</param>
        /// <param name="pinMoveCallback">The pin point move callback.</param>
        /// <param name="szPinOffset">The pin point offset.</param>
        /// <param name="pinOffsetCallback">The pin point offset callback.</param>
        /// <param name="szSize">Size of the node.</param>
        /// <param name="szSizeCallback">The size callback.</param>
        internal BoundsInfo(
                           PointF ptPinPoint, 
                           MoveCallback pinMoveCallback,
                           SizeF szPinOffset, 
                           PinOffsetCallback pinOffsetCallback,
                           SizeF szSize, 
                           SizeCallback szSizeCallback)
            : this(ptPinPoint, szPinOffset, szSize, 6)
        {
            m_callbackMove = pinMoveCallback;
            m_callbackSize = szSizeCallback;
            m_callbackPinOffset = pinOffsetCallback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundsInfo"/> class from known 
        /// pin point position, size and pin point offset.
        /// </summary>
        /// <param name="ptPinPoint">The pin point position.</param>
        /// <param name="szPinOffset">The pin point offset.</param>
        /// <param name="szSize">Size of the node.</param>
        /// <param name="fHitTestPadding">The hit test padding.</param>
        internal BoundsInfo(PointF ptPinPoint, SizeF szPinOffset, SizeF szSize, float fHitTestPadding)
        {
            m_ptPinPoint = m_ptUnitDependentPinValue = ptPinPoint;
            m_szPinOffset = m_szUnitDependentPinOffsetValue = szPinOffset;
            m_szSize = m_szUnitDependentSizeValue = szSize;
            m_fHitTestPadding = m_fUnitDependentHitTestPadding = fHitTestPadding;
            m_unitMeasure = MeasureUnits.Pixel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundsInfo"/> class from source instance.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public BoundsInfo(BoundsInfo src)
        {
            m_scale = src.m_scale;
            m_fHitTestPadding = src.m_fHitTestPadding;
            m_ptPinPoint = src.m_ptPinPoint;
            m_szPinOffset = src.m_szPinOffset;
            m_szSize = src.m_szSize;
            m_unitMeasure = src.m_unitMeasure;
            m_fUnitDependentHitTestPadding = src.m_fUnitDependentHitTestPadding;
            m_ptUnitDependentPinValue = src.m_ptUnitDependentPinValue;
            m_szUnitDependentPinOffsetValue = src.m_szUnitDependentPinOffsetValue;
            m_szUnitDependentSizeValue = src.m_szUnitDependentSizeValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundsInfo"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        private BoundsInfo(SerializationInfo info, StreamingContext context)
        {
            m_fHitTestPadding = (float)info.GetValue("hitTestPadding", typeof(float));
            m_ptPinPoint = (PointF)info.GetValue("pinPoint", typeof(PointF));
            m_szPinOffset = (SizeF)info.GetValue("pinOffset", typeof(SizeF));
            m_szSize = (SizeF)info.GetValue("size", typeof(SizeF));
            m_unitMeasure = (MeasureUnits)info.GetValue("unit", typeof(MeasureUnits));

            // load node scale if exist, otherwise it generate inside NodeScale property
            try
            {
                m_scale = (PageScale)info.GetValue("nodeScale", typeof(PageScale));
            }
            catch 
            { 
            }

            // Update unit dependent values
            m_fUnitDependentHitTestPadding = MeasureUnitsConverter.FromPixelX(m_fHitTestPadding, m_unitMeasure);
            m_ptUnitDependentPinValue = MeasureUnitsConverter.FromPixels(m_ptPinPoint, m_unitMeasure);
            m_szUnitDependentSizeValue = MeasureUnitsConverter.FromPixels(m_szSize, m_unitMeasure);
            m_szUnitDependentPinOffsetValue = MeasureUnitsConverter.FromPixels(m_szPinOffset, m_unitMeasure);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether node size is changing.
        /// </summary>
        /// <value><c>true</c> if node size is changing ; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// SizeChanged/PinPointChanged/PinOffsetChanged events will not be raised if it is true
        /// </remarks>
        public bool IsResizing
        {
            get
            {
                return m_bIsResizing;
            }
            set
            {
                if (value != m_bIsResizing && OnPropertyChanging(DPN.IsResizing, value))
                {
                    //assign new value
                    m_bIsResizing = value;
                    OnPropertyChanged(DPN.IsResizing);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connector's segment is changing.
        /// </summary>
        /// <value><c>true</c> if connector's segment is changing ; otherwise, <c>false</c>.</value>
        public bool IsSegmentChanging
        {
            get
            {
                return m_bIsSegmentChanging;
            }
            set
            {
                if (value != m_bIsSegmentChanging && OnPropertyChanging(DPN.IsSegmentChanging, value))
                {
                    //assign new value
                    m_bIsSegmentChanging = value;
                    OnPropertyChanged(DPN.IsSegmentChanging);
                }
            }
        }

        /// <summary>
        /// Gets or sets the node scale.
        /// </summary>
        /// <value>The node scale.</value>
        [Browsable(false)]
        public PageScale NodeScale
        {
            get
            {
                if (m_scale == null)
                {
                    m_scale = new PageScale();
                    m_scale.UpdateServiceReferences(this.PropertyObserber as IServiceReferenceProvider);
                }

                return m_scale;
            }
            set
            {
                if (m_scale != value && OnPropertyChanging(DPN.NodeScale, value))
                {
                    if (m_scale != null)
                        m_scale.UpdateServiceReferences(null);

                    HistoryManager history = this.HistoryService;

                    if (history != null)
                        history.StartAtomicAction("NodeScale change");

                    RecordPropertyChanged(DPN.NodeScale);

                    // set new value
                    m_scale = value;
                    m_scale.UpdateServiceReferences(this.PropertyObserber as IServiceReferenceProvider);

                    if (history != null)
                        history.EndAtomicAction();

                    OnPropertyChanged(DPN.NodeScale);
                }
            }
        }

        /// <summary>
        /// Gets or sets the measure units that transformation parameters are saved in.
        /// </summary>
        /// <value>The unit.</value>
        [Browsable(false)]
        public MeasureUnits Unit
        {
            get 
            { 
                return m_unitMeasure; 
            }
            set
            {
                if (m_unitMeasure != value && OnPropertyChanging(DPN.Unit, value))
                {
                    // Update unit dependent values
                    if (value != MeasureUnits.Pixel)
                    {
                        m_fUnitDependentHitTestPadding = MeasureUnitsConverter.FromPixelX(m_fHitTestPadding, value);
                        m_ptUnitDependentPinValue = MeasureUnitsConverter.FromPixels(m_ptPinPoint, value);
                        m_szUnitDependentSizeValue = MeasureUnitsConverter.FromPixels(m_szSize, value);
                        m_szUnitDependentPinOffsetValue = MeasureUnitsConverter.FromPixels(m_szPinOffset, value);
                    }

                    RecordPropertyChanged(DPN.Unit);
                    
                    // assign new value
                    m_unitMeasure = value;

                    OnPropertyChanged(DPN.Unit);
                }
            }
        }

        /// <summary>
        /// Gets or sets the pin point position.
        /// </summary>
        /// <value>The pin point.</value>
        private PointF PinPoint
        {
            get 
            { 
                return m_ptPinPoint; 
            }
            set
            {
                // get offset
                float fPixelOffsetX = value.X - m_ptPinPoint.X;
                float fPixelOffsetY = value.Y - m_ptPinPoint.Y;

                if (m_ptPinPoint != value && OnPinPointChanging(fPixelOffsetX, fPixelOffsetY))
                {
                    HistoryManager mngHistory = this.HistoryService;

                    if (mngHistory != null)
                        mngHistory.StartAtomicAction("Pin Point change");

                    ScaleValue(ref fPixelOffsetX, ref fPixelOffsetY, MeasureUnits.Pixel, false, true);
                    RecordMove(DPN.PinPoint, new SizeF(fPixelOffsetX, fPixelOffsetY));

                    // assign new unit independent value
                    m_ptPinPoint = value;
                    
                    // assign unit dependent value
                    m_ptUnitDependentPinValue = MeasureUnitsConverter.FromPixels(value, this.Unit);

                    if (m_callbackMove != null)
                    {
                        m_callbackMove(fPixelOffsetX, fPixelOffsetY);
                    }

                    if (mngHistory != null)
                        mngHistory.EndAtomicAction();
                    if(!IsResizing)
                        OnPinPointChanged(fPixelOffsetX, fPixelOffsetY);
                }
            }
        }

        /// <summary>
        /// Gets or sets the pin point offset.
        /// </summary>
        /// <value>The pin point offset.</value>
        private SizeF PinOffset
        {
            get 
            { 
                return m_szPinOffset; 
            }
            set
            {
                float fOffsetX = m_szPinOffset.Width - value.Width;
                float fOffsetY = m_szPinOffset.Height - value.Height;

                if (m_szPinOffset != value && OnPinOffsetChanging(fOffsetX, fOffsetY))
                {
                    HistoryManager mngHistory = this.HistoryService;

                    if (mngHistory != null)
                        mngHistory.StartAtomicAction("Pin Offset change");

                    RecordPropertyChanged(DPN.PinOffset);

                    // save old value
                    SizeF szOldPinOffset = m_szPinOffset;
                    
                    // assign new unit independent value
                    m_szPinOffset = value;
                    
                    // assign unit dependent value
                    m_szUnitDependentPinOffsetValue = MeasureUnitsConverter.FromPixels(value, this.Unit);

                    if (m_callbackPinOffset != null)
                    {
                        m_callbackPinOffset(szOldPinOffset, value);
                    }

                    if (mngHistory != null)
                        mngHistory.EndAtomicAction();
                    if(!IsResizing)
                        OnPinOffsetChanged(fOffsetX, fOffsetY);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        private SizeF Size
        {
            get 
            { 
                return m_szSize; 
            }
            set
            {
                float fSizeOffsetX = value.Width - m_szSize.Width;
                float fSizeOffsetY = value.Height - m_szSize.Height;

                if (m_szSize != value && OnSizeChanging(fSizeOffsetX, fSizeOffsetY) && OnPropertyChanging(DPN.Size, value))
                {
                    HistoryManager history = this.HistoryService;

                    if (history != null)
                        history.StartAtomicAction("Size change");

                    RecordPropertyChanged(DPN.Size);

                    SizeF szOldSize = m_szSize;
                   
                    // assign new unit independent value
                    m_szSize = value;

                    // stop history
                    if (history != null)
                        history.Pause();

                    // update pin offset
                    UpdatePinOffset(szOldSize, m_szSize, m_unitMeasure);

                    // restore history
                    if (history != null)
                        history.Resume();

                    if (m_callbackSize != null)
                    {
                        m_callbackSize(szOldSize, value);
                    }

                    // assign unit dependent value
                    m_szUnitDependentSizeValue = MeasureUnitsConverter.FromPixels(value, this.Unit);

                    if (history != null)
                        history.EndAtomicAction();
                    if(!IsResizing)
                        OnSizeChanged(fSizeOffsetX, fSizeOffsetY);
                    OnPropertyChanged(DPN.Size);
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Updates the service references. Update reference to document eventSink and nodeScale.
        /// </summary>
        /// <remarks>
        /// Update m_eventSink member reference and update nodeScale object references.
        /// </remarks>
        /// <param name="provider">The service provider.</param>
        public override void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            base.UpdateServiceReferences(provider);

            if (provider != null)
            {
                m_eventSink = (DocumentEventSink)provider.ProvideServiceReference(typeof(DocumentEventSink).TypeHandle);
            }
            else
            {
                m_eventSink = null;
            }

            this.NodeScale.UpdateServiceReferences(provider);
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Name of Bounds info.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.BoundsInfo;
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("hitTestPadding", m_fHitTestPadding);
            info.AddValue("pinPoint", m_ptPinPoint);
            info.AddValue("pinOffset", m_szPinOffset);
            info.AddValue("size", m_szSize);
            info.AddValue("unit", m_unitMeasure);

            if (m_scale != null)
                info.AddValue("nodeScale", m_scale);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new BoundsInfo(this);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Gets the upper left point in given units that calculated from PinPoint Position and PinOffset.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <returns>The point.</returns>
        public PointF GetUpperLeftPoint(MeasureUnits unit)
        {
            PointF ptPinPoint;
            SizeF szPinOffset;

            if (unit == MeasureUnits.Pixel)
            {
                ptPinPoint = this.PinPoint;
                szPinOffset = this.PinOffset;
            }
            else if (unit == m_unitMeasure)
            {
                ptPinPoint = m_ptPinPoint;
                szPinOffset = m_szPinOffset;
            }
            else
            {
                ptPinPoint = MeasureUnitsConverter.FromPixels(this.PinPoint, unit);
                szPinOffset = MeasureUnitsConverter.FromPixels(this.PinOffset, unit);
            }

            PointF ptUpperLeft = new PointF(ptPinPoint.X - szPinOffset.Width, ptPinPoint.Y - szPinOffset.Height);

            if (!ptUpperLeft.IsEmpty)
            {
                ScaleValue(ref ptPinPoint, unit, false, true);
                ScaleValue(ref szPinOffset, unit, false, false);
            }

            ptUpperLeft = new PointF(ptPinPoint.X - szPinOffset.Width, ptPinPoint.Y - szPinOffset.Height);
            return ptUpperLeft;
        }

        /// <summary>
        /// Gets the hit test padding in given units.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <returns>The hit test padding value.</returns>
        public float GetHitTestPadding(MeasureUnits unit)
        {
            float fToReturn;

            if (unit == MeasureUnits.Pixel)
            {
                fToReturn = m_fHitTestPadding;
            }
            else if (unit == m_unitMeasure)
            {
                fToReturn = m_fUnitDependentHitTestPadding;
            }
            else
            {
                fToReturn = MeasureUnitsConverter.FromPixelX(m_fHitTestPadding, unit);
            }

            float fTemp = 0;
            ScaleValue(ref fTemp, ref fToReturn, unit, false, false);

            return fToReturn;
        }

        /// <summary>
        /// Gets the pin point position in given units.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <returns>The pin point.</returns>
        public PointF GetPinPoint(MeasureUnits unit)
        {
            PointF ptToReturn;

            if (unit == MeasureUnits.Pixel)
            {
                ptToReturn = this.PinPoint;
            }
            else if (unit == m_unitMeasure)
            {
                ptToReturn = m_ptUnitDependentPinValue;
            }
            else
            {
                ptToReturn = MeasureUnitsConverter.FromPixels(this.PinPoint, unit);
            }

            ScaleValue(ref ptToReturn, unit, false, true);

            return ptToReturn;
        }

        /// <summary>
        /// Gets the size in given units.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <returns>The size.</returns>
        public SizeF GetSize(MeasureUnits unit)
        {
            SizeF szToReturn;

            if (unit == MeasureUnits.Pixel)
            {
                szToReturn = this.Size;
            }
            else if (unit == m_unitMeasure)
            {
                szToReturn = m_szUnitDependentSizeValue;
            }
            else
            {
                szToReturn = MeasureUnitsConverter.FromPixels(this.Size, unit);
            }

            ScaleValue(ref szToReturn, unit, false, false);

            return szToReturn;
        }

        /// <summary>
        /// Gets the pin offset in given units.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <returns>The pin offset.</returns>
        public SizeF GetPinOffset(MeasureUnits unit)
        {
            SizeF szToReturn;

            if (unit == MeasureUnits.Pixel)
            {
                szToReturn = this.PinOffset;
            }
            else if (unit == m_unitMeasure)
            {
                szToReturn = m_szUnitDependentPinOffsetValue;
            }
            else
            {
                szToReturn = MeasureUnitsConverter.FromPixels(this.PinOffset, unit);
            }

            ScaleValue(ref szToReturn, unit, false, false);

            return szToReturn;
        }

        /// <summary>
        /// Sets the new hit test padding value.
        /// </summary>
        /// <param name="fValue">The new value to set.</param>
        /// <param name="unit">The measure unit of new value.</param>
        public void SetHitTestPadding(float fValue, MeasureUnits unit)
        {
            ScaleValue(ref fValue, ref fValue, unit, true, false);

            if (unit == MeasureUnits.Pixel)
            {
                // assign new unit independent value
                m_fHitTestPadding = fValue;
                
                // update unit dependent value
                if (unit == MeasureUnits.Pixel)
                {
                    m_fUnitDependentHitTestPadding = fValue;
                }
                else
                {
                    m_fUnitDependentHitTestPadding = MeasureUnitsConverter.Convert(fValue, this.Unit, unit);
                }
            }
            else if (unit == m_unitMeasure)
            {
                // assign unit dependent value
                m_fUnitDependentHitTestPadding = fValue;
                
                // update unit independent value
                m_fHitTestPadding = MeasureUnitsConverter.ToPixelX(fValue, unit);
            }
        }

        /// <summary>
        /// Sets the new pin point position.
        /// </summary>
        /// <param name="ptValue">The new point point position.</param>
        /// <param name="unit">The measure unit of new value.</param>
        public void SetPinPoint(PointF ptValue, MeasureUnits unit)
        {
            ScaleValue(ref ptValue, unit, true, true);

            if (unit == MeasureUnits.Pixel)
            {
                // assign new unit independent value
                this.PinPoint = ptValue;
            }
            else
            {
                // update unit independent value
                this.PinPoint = MeasureUnitsConverter.ToPixels(ptValue, unit);
            }
        }

        /// <summary>
        /// Sets the new size.
        /// </summary>
        /// <param name="szValue">The new size value.</param>
        /// <param name="unit">The measure unit of new value.</param>
        public void SetSize(SizeF szValue, MeasureUnits unit)
        {
            ScaleValue(ref szValue, unit, true, false);

            if (unit == MeasureUnits.Pixel)
            {
                // assign new unit independent value
                this.Size = szValue;
                
                // update unit dependent value
            }
            else
            {
                this.Size = MeasureUnitsConverter.ToPixels(szValue, unit);
            }
        }

        /// <summary>
        /// Sets the new pin point offset.
        /// </summary>
        /// <param name="szValue">The new pin point offset.</param>
        /// <param name="unit">The measure unit of new value.</param>
        public void SetPinOffset(SizeF szValue, MeasureUnits unit)
        {
            ScaleValue(ref szValue, unit, true, false);

            if (unit == MeasureUnits.Pixel)
            {
                // assign new unit independent value
                this.PinOffset = szValue;
            }
            else
            {
                this.PinOffset = MeasureUnitsConverter.ToPixels(szValue, unit);
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Scales the value to current scale factor.
        /// </summary>
        /// <param name="szToScale">The size to scale.</param>
        /// <param name="units">The current measure units.</param>
        /// <param name="bSubtract">if set to <c>true</c> to subtract scale.</param>
        /// <param name="bParentOnly">if set to <c>true</c> to append parents scale only.</param>
        private void ScaleValue(ref SizeF szToScale, MeasureUnits units, bool bSubtract, bool bParentOnly)
        {
            if (!szToScale.IsEmpty)
            {
                float fWidth = szToScale.Width;
                float fHeight = szToScale.Height;

                ScaleValue(ref fWidth, ref fHeight, units, bSubtract, bParentOnly);

                szToScale.Width = fWidth;
                szToScale.Height = fHeight;
            }
        }

        /// <summary>
        /// Scales the value to current scale factor.
        /// </summary>
        /// <param name="ptToScale">The point to scale.</param>
        /// <param name="units">The current measure units.</param>
        /// <param name="bSubtract">if set to <c>true</c> to subtract scale.</param>
        /// <param name="bParentOnly">if set to <c>true</c> to append parents scale only].</param>
        private void ScaleValue(ref PointF ptToScale, MeasureUnits units, bool bSubtract, bool bParentOnly)
        {
            if (!ptToScale.IsEmpty)
            {
                float fX = ptToScale.X;
                float fY = ptToScale.Y;

                ScaleValue(ref fX, ref fY, units, bSubtract, bParentOnly);

                ptToScale.X = fX;
                ptToScale.Y = fY;
            }
        }

        /// <summary>
        /// Scales the value to current scale factor.
        /// </summary>
        /// <param name="x">The X point position.</param>
        /// <param name="y">The Y point position.</param>
        /// <param name="units">The current measure units.</param>
        /// <param name="bSubtract">if set to <c>true</c> to subtract scale.</param>
        /// <param name="bParentOnly">if set to <c>true</c> to append parents scale only.</param>
        private void ScaleValue(ref float x, ref float y, MeasureUnits units, bool bSubtract, bool bParentOnly)
        {
            if (x != 0 && y != 0)
            {
                float fFactor = bParentOnly ? 1f : this.NodeScale.GetScaleFactor(units);
                Node node = this.PropertyObserber as Node;

                if (node != null)
                {
                    Node parent = node.Parent as Node;
                    Model model = node.Root;

                    while (parent != null)
                    {
                        fFactor *= parent.NodeScale.GetScaleFactor(units);
                        parent = parent.Parent as Node;
                    }

                    if (model != null)
                        fFactor *= model.DocumentScale.GetScaleFactor(units);
                }

                x = bSubtract ? x / fFactor : x * fFactor;
                y = bSubtract ? y / fFactor : y * fFactor;
            }
        }

        /// <summary>
        /// Records the move operation to history.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="szMoveOffset">The move offset.</param>
        private void RecordMove(string strPropertyName, SizeF szMoveOffset)
        {
            if (this.HistoryService != null)
            {
                this.HistoryService.RecordMovePinPoint(
                    (IPropertyContainer)m_propertyObserver,
                    this.FullContainerName, 
                    strPropertyName, 
                    szMoveOffset);
            }
        }

        /// <summary>
        /// Called when pin point posiytion changing.
        /// </summary>
        /// <param name="fOffsetX">The offset by X axis.</param>
        /// <param name="fOffsetY">The offset by Y axis.</param>
        /// <returns><b>True</b> if new PinPoint position can be set, otherwise  - <b>false</b>.</returns>
        private bool OnPinPointChanging(float fOffsetX, float fOffsetY)
        {
            bool bSuccess = true;

            if (m_eventSink != null && this.PropertyObserber != null)
            {
                SizeF szOffset = new SizeF(fOffsetX, fOffsetY);
                bSuccess = m_eventSink.RaisePinPointChanging(new PinPointChangingEventArgs((Node)PropertyObserber, szOffset));
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when pin point position is changed.
        /// </summary>
        /// <param name="fOffsetX">The offset by X axis.</param>
        /// <param name="fOffsetY">The offset by Y axis.</param>
        private void OnPinPointChanged(float fOffsetX, float fOffsetY)
        {
            if (m_eventSink != null && this.PropertyObserber != null)
            {
                SizeF szOffset = new SizeF(fOffsetX, fOffsetY);
                m_eventSink.RaisePinPointChanged(new PinPointChangedEventArgs((Node)PropertyObserber, szOffset));
            }
        }

        /// <summary>
        /// Called when pin point offset changing.
        /// </summary>
        /// <param name="fOffsetX">The offset by X axis.</param>
        /// <param name="fOffsetY">The offset by Y axis.</param>
        /// <returns><b>True</b> if new PointPointOffset value can be set, otherwise - <b>false</b>.</returns>
        private bool OnPinOffsetChanging(float fOffsetX, float fOffsetY)
        {
            bool bSuccess = true;

            if (m_eventSink != null && this.PropertyObserber != null)
            {
                SizeF szOffset = new SizeF(fOffsetX, fOffsetY);
                bSuccess = m_eventSink.RaisePinOffsetChanging(new PinOffsetChangingEventArgs((Node)PropertyObserber, szOffset));
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when pin point offset is changed.
        /// </summary>
        /// <param name="fOffsetX">The offset by X axis.</param>
        /// <param name="fOffsetY">The offset by Y axis.</param>
        private void OnPinOffsetChanged(float fOffsetX, float fOffsetY)
        {
            if (m_eventSink != null && this.PropertyObserber != null)
            {
                SizeF szOffset = new SizeF(fOffsetX, fOffsetY);
                m_eventSink.RaisePinOffsetChanged(new PinOffsetChangedEventArgs((Node)PropertyObserber, szOffset));
            }
        }

        /// <summary>
        /// Called when size changing.
        /// </summary>
        /// <param name="fOffsetX">The offset by X axis.</param>
        /// <param name="fOffsetY">The offset by Y axis.</param>
        /// <returns><b>True</b> if new size value can be set, otherwise - <b>false</b>.</returns>
        private bool OnSizeChanging(float fOffsetX, float fOffsetY)
        {
            bool bSuccess = true;

            if (m_eventSink != null && this.PropertyObserber != null)
            {
                SizeF szOffset = new SizeF(fOffsetX, fOffsetY);
                bSuccess = m_eventSink.RaiseSizeChanging(new SizeChangingEventArgs((Node)PropertyObserber, szOffset));
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when size is changed.
        /// </summary>
        /// <param name="fOffsetX">The offset by X axis.</param>
        /// <param name="fOffsetY">The offset by Y axis.</param>
        private void OnSizeChanged(float fOffsetX, float fOffsetY)
        {
            if (m_eventSink != null && this.PropertyObserber != null)
            {
                SizeF szOffset = new SizeF(fOffsetX, fOffsetY);
                m_eventSink.RaiseSizeChanged(new SizeChangedEventArgs((Node)PropertyObserber, szOffset));
            }
        }

        /// <summary>
        /// Updates the pin point offset to size changes.
        /// </summary>
        /// <param name="szOldSize">Old Size value.</param>
        /// <param name="szNewSize">New Size value.</param>
        /// <param name="unit">The size measure unit.</param>
        /// <remarks>
        /// Update pin point offset using size change factor to resize node in all directions.
        /// </remarks>
        private void UpdatePinOffset(SizeF szOldSize, SizeF szNewSize, MeasureUnits unit)
        {
            SizeF szPinOffsetUnitDependent = GetPinOffset(unit);

            // prevent divide by zero
            szOldSize.Width = (szOldSize.Width == 0) ? 1 : szOldSize.Width;
            szOldSize.Height = (szOldSize.Height == 0) ? 1 : szOldSize.Height;

            // calc new offset
            float fOffsetX = (szNewSize.Width - szOldSize.Width) * (szPinOffsetUnitDependent.Width / szOldSize.Width);
            float fOffsetY = (szNewSize.Height - szOldSize.Height) * (szPinOffsetUnitDependent.Height / szOldSize.Height);

            // apply new offset
            szPinOffsetUnitDependent.Width += fOffsetX;
            szPinOffsetUnitDependent.Height += fOffsetY;

            SetPinOffset(szPinOffsetUnitDependent, unit);
        }
        #endregion
    }
}
