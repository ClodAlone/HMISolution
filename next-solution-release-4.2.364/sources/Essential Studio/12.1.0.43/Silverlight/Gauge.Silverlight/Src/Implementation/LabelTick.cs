#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents Label Tick
    /// </summary>
    public class LabelTick : LabelTickSet
    {        

        #region Private members

        /// <summary>
        /// The text block used to draw the text.
        /// </summary>
        private TextBlock mtextBlock;

        //internal string tooltip;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.LabelTick">LabelTick</see> class.
        /// </summary>
        public LabelTick()
        {
            DefaultStyleKey = typeof(LabelTick);           
        }
        
        #endregion

        #region Events

        #endregion

        #region DP getters & setters

       
        //public string Text
        //{
        //    get
        //    {
        //        return (string)GetValue(TextProperty);
        //    }

        //    set
        //    {
        //        SetValue(TextProperty, value);
        //    }
        //}

       
        //internal double XOffset
        //{
        //    get
        //    {
        //        return (double)GetValue(XOffsetProperty);
        //    }

        //    set
        //    {
        //        SetValue(XOffsetProperty, value);
        //    }
        //}

        
        //internal double YOffset
        //{
        //    get
        //    {
        //        return (double)GetValue(YOffsetProperty);
        //    }

        //    set
        //    {
        //        SetValue(YOffsetProperty, value);
        //    }
        //}
        
        #endregion

        #region Overrides
        
        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.mtextBlock = this.GetTemplateChild("TextBlock") as TextBlock;
            ToolTip t = new ToolTip()
            {
                Content=this.tooltip
            };
            if (this.tooltip != null)
            {
                ToolTipService.SetToolTip(this.mtextBlock, t);
            }
            else
            {
                ToolTipService.SetToolTip(this.mtextBlock, null);
            }

            this.RefreshLabelTick();
            this.IsTabStop = false;
        }

        /// <summary>
        /// Method called when Label Foreground is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details such as old value and new value.</param>
        protected override void OnLabelForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnLabelForegroundChanged(e);
            this.RefreshLabelTick();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Refreshs Label Tick set
        /// </summary>
        internal void RefreshLabelTick()
        {
            if (this.mtextBlock != null)
            {
                if (this.GaugeElementParent is LabelTickSet)
                {
                    LabelTickSet tickSet = this.GaugeElementParent as LabelTickSet;
                    this.mtextBlock.Text = this.Text;
                    this.mtextBlock.FontFamily = tickSet.FontFamily;
                    this.mtextBlock.FontStyle = tickSet.FontStyle;
                    this.mtextBlock.FontSize = tickSet.FontSize;
                    this.mtextBlock.FontWeight = tickSet.FontWeight;
                    //this.mtextBlock.Foreground = this.LabelForeground;
                    ToolTip t = new ToolTip()
                    {
                        Content = this.tooltip
                    };
                    if (this.tooltip != null)
                    {
                        ToolTipService.SetToolTip(this.mtextBlock, t);
                    }
                    else
                    {
                        ToolTipService.SetToolTip(this.mtextBlock, null);
                    }
                }
            }
        }

        /// <summary>
        /// Measures Size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <returns>Returns the size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            if (this.mtextBlock != null && this.mtextBlock.Text != String.Empty)
            {
                size = new Size(this.mtextBlock.ActualWidth, this.mtextBlock.ActualHeight);
            }

            return size;
        }

        ///// <summary>
        ///// Updates property value cache and raises <see cref="TextChanged"/> event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.TextChanged != null)
        //    {
        //        this.TextChanged(this, e);
        //    }
        //}
        
        ///// <summary>
        ///// Updates property value cache and raises <see cref="XOffsetChanged"/> event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //protected virtual void OnXOffsetChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.XOffsetChanged != null)
        //    {
        //        this.XOffsetChanged(this, e);
        //    }
        //}
        
        ///// <summary>
        ///// Updates property value cache and raises <see cref="YOffsetChanged"/> event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //protected virtual void OnYOffsetChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.YOffsetChanged != null)
        //    {
        //        this.YOffsetChanged(this, e);
        //    }
        //}

        ///// <summary>
        ///// Calls OnYOffsetChanged method of the instance, notifies of the depencency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //private static void OnYOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    LabelTick instance = (LabelTick)d;
        //    instance.OnYOffsetChanged(e);
        //}

        ///// <summary>
        ///// Calls OnXOffsetChanged method of the instance, notifies of the depencency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //private static void OnXOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    LabelTick instance = (LabelTick)d;
        //    instance.OnXOffsetChanged(e);
        //}

        ///// <summary>
        ///// Calls OnTextChanged method of the instance, notifies of the depencency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    LabelTick instance = (LabelTick)d;
        //    instance.OnTextChanged(e);
        //}

        #endregion
    }
}
