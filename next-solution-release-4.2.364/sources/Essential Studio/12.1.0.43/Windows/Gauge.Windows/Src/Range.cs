#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Gauge
{
    public class Range
    {
        public Range() { }
        /// <summary>
        /// Affected GaugeRange
        /// </summary>
        /// <param name="color"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="height"></param>
        /// <param name="rangePlacement"></param>        
        public Range(Color color, Single startValue, Single endValue, Int32 height, TickPlacement rangePlacement)
        {
            Color = color;
            _StartValue = startValue;
            _EndValue = endValue;
            Height = height;
            RangePlacement = rangePlacement;
        }
        /// <summary>
        /// Gets or Sets the instance range name
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.DisplayName("(Name)"),
        System.ComponentModel.Description("Instance Name.")]
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the instance range name
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Boolean InRange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private RadialGauge Owner;
        /// <summary>
        /// Specifies the owner of the Range
        /// </summary>
        /// <param name="value"></param>
        [System.ComponentModel.Browsable(false)]
        public void SetOwner(RadialGauge value) { Owner = value; }
        /// <summary>
        /// Specifies the notify owner of the Range
        /// </summary>
        private void NotifyOwner() { if (Owner != null) Owner.RepaintControl(); }
        /// <summary>
        /// Gets or Sets the color of the Range.
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("The color of the range.")]
        public Color Color { get { return _Color; } set { _Color = value; NotifyOwner(); } }
        private Color _Color=Color.FromArgb(0, 255, 0);
        /// <summary>
        /// Gets or Sets the start value of the Range.
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Limits"),
        System.ComponentModel.Description("The start value of the range, must be less than RangeEndValue.")]
        public Single StartValue
        {
            get { return _StartValue; }
            set { if (value < _EndValue) { _StartValue = value; NotifyOwner(); } }
        }
        private Single _StartValue;
        /// <summary>
        /// Gets or Sets the end  value of the Range.
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Limits"),
        System.ComponentModel.Description("The end value of the range. Must be greater than RangeStartValue.")]
        public Single EndValue
        {
            get { return _EndValue; }
            set { if (value > _StartValue) { _EndValue = value; NotifyOwner(); } }
        }
        private Single _EndValue;
        /// <summary>
        /// Gets or Sets the inner radius of the range
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("The inner radius of the range.")]
        public Int32 Height
        {
            get { return _Height; }
            set { if (value > 0) { _Height = value; NotifyOwner(); } }
        }
        private Int32 _Height = 5;
        /// <summary>
        /// Gets or Sets the outer radius of the range
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("The outer radius of the range.")]
        public TickPlacement RangePlacement
        {
            get { return _RangePlacement; }
            set { _RangePlacement = value; NotifyOwner(); }
        }
        private TickPlacement _RangePlacement = TickPlacement.Inside;
    }
    public class LinearRange
    {
        public LinearRange() { }
        /// <summary>
        /// Affected GaugeRange
        /// </summary>
        /// <param name="color"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="height"></param>
        /// <param name="rangePlacement"></param>
        public LinearRange(Color color, Single startValue, Single endValue, Int32 height, TickPlacement rangePlacement)
        {
            Color = color;
            _StartValue = startValue;
            _EndValue = endValue;
            Height = height;
            RangePlacement = rangePlacement;
        }
        /// <summary>
        /// Gets or Sets the instance range name
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.DisplayName("(Name)"),
        System.ComponentModel.Description("Instance Name.")]
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the instance range name
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Boolean InRange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private LinearGauge Owner;
        /// <summary>
        /// Specifies the owner of the Range
        /// </summary>
        /// <param name="value"></param>
        [System.ComponentModel.Browsable(false)]
        public void SetOwner(LinearGauge value) { Owner = value; }
        /// <summary>
        /// Specifies the notify owner of the Range
        /// </summary>
        private void NotifyOwner() { if (Owner != null) Owner.RepaintControl(); }
        /// <summary>
        /// Gets or Sets the color of the Range.
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("The color of the range.")]
        public Color Color { get { return _Color; } set { _Color = value; NotifyOwner(); } }
        private Color _Color = Color.FromArgb(0, 255, 0);
        /// <summary>
        /// Gets or Sets the start value of the Range.
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Limits"),
        System.ComponentModel.Description("The start value of the range, must be less than RangeEndValue.")]
        public Single StartValue
        {
            get { return _StartValue; }
            set { if (value < _EndValue) { _StartValue = value; NotifyOwner(); } }
        }
        private Single _StartValue;
        /// <summary>
        /// Gets or Sets the end  value of the Range.
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Limits"),
        System.ComponentModel.Description("The end value of the range. Must be greater than RangeStartValue.")]
        public Single EndValue
        {
            get { return _EndValue; }
            set { if (value > _StartValue) { _EndValue = value; NotifyOwner(); } }
        }
        private Single _EndValue;
        /// <summary>
        /// Gets or Sets the inner radius of the range
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("The inner radius of the range.")]
        public Int32 Height
        {
            get { return _Height; }
            set { if (value > 0) { _Height = value; NotifyOwner(); } }
        }
        private Int32 _Height = 5;
        /// <summary>
        /// Gets or Sets the outer radius of the range
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("The outer radius of the range.")]
        public TickPlacement RangePlacement
        {
            get { return _RangePlacement; }
            set { _RangePlacement = value; NotifyOwner(); }
        }
        private TickPlacement _RangePlacement = TickPlacement.Inside;
    }
}
