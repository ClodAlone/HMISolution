#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class SliderPropertiesBuilder
    {
        public Slider slider;

        public SliderPropertiesBuilder(Slider slider)
        { this.slider = new Slider(slider.ID, slider.SliderModel); }
        
        public SliderPropertiesBuilder()
        {
        }
        //int  Values
        public SliderPropertiesBuilder AnimationSpeed(int animationSpeed)
        {
            slider.SliderModel.AnimationSpeed = animationSpeed;
            return this;
        }
        public SliderPropertiesBuilder StartValue(int startValue)
        {
            slider.SliderModel.StartValue = startValue;
            return this;
        }
        public SliderPropertiesBuilder EndValue(int endValue)
        {
            slider.SliderModel.EndValue = endValue;
            return this;
        }
        public SliderPropertiesBuilder Step(int step)
        {
            slider.SliderModel.Step = step;
            return this;
        }
        public SliderPropertiesBuilder LargeStep(int largeStep)
        {
            slider.SliderModel.LargeStep = largeStep;
            return this;
        }
        public SliderPropertiesBuilder SmallStep(int smallStep)
        {
            slider.SliderModel.SmallStep = smallStep;
            return this;
        }
        //Boolean values
        public SliderPropertiesBuilder Animate()
        {
            slider.SliderModel.Animate = true;
            return this;
        }
        public SliderPropertiesBuilder Animate(bool animate)
        {
            slider.SliderModel.Animate = animate;
            return this;
        }
        public SliderPropertiesBuilder ShowTooltip()
        {
            slider.SliderModel.ShowTooltip = true;
            return this;
        }
        public SliderPropertiesBuilder ShowTooltip(bool showTooltip)
        {
            slider.SliderModel.ShowTooltip = showTooltip;
            return this;
        }
        public SliderPropertiesBuilder RoundedCorner()
        {
            slider.SliderModel.RoundedCorner = true;
            return this;
        }
        public SliderPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            slider.SliderModel.RoundedCorner = roundedCorner;
            return this;
        }
        public SliderPropertiesBuilder ReadOnly()
        {
            slider.SliderModel.ReadOnly = true;
            return this;
        }
        public SliderPropertiesBuilder ReadOnly(bool readOnly)
        {
            slider.SliderModel.ReadOnly = readOnly;
            return this;
        }
        public SliderPropertiesBuilder Rtl()
        {
            slider.SliderModel.Rtl = true;
            return this;
        }
        public SliderPropertiesBuilder Rtl(bool rtl)
        {
            slider.SliderModel.Rtl = rtl;
            return this;
        }
        public SliderPropertiesBuilder Enabled()
        {
            slider.SliderModel.Enabled = true;
            return this;
        }
        public SliderPropertiesBuilder Enabled(bool enabled)
        {
            slider.SliderModel.Enabled = enabled;
            return this;
        }
        public SliderPropertiesBuilder ShowScale()
        {
            slider.SliderModel.ShowScale = true;
            return this;
        }
        public SliderPropertiesBuilder ShowScale(bool showScale)
        {
            slider.SliderModel.ShowScale = showScale;
            return this;
        }
        public SliderPropertiesBuilder ShowSmallTicks()
        {
            slider.SliderModel.ShowSmallTicks = true;
            return this;
        }
        public SliderPropertiesBuilder ShowSmallTicks(bool showSmallTicks)
        {
            slider.SliderModel.ShowSmallTicks = showSmallTicks;
            return this;
        }
        public SliderPropertiesBuilder Persist()
        {
            slider.SliderModel.Persist = true;
            return this;
        }
        public SliderPropertiesBuilder Persist(bool persist)
        {
            slider.SliderModel.Persist = persist;
            return this;
        }
        //Enumvalues
        public SliderPropertiesBuilder Orientation(Orientation orientation)
        {
            slider.SliderModel.Orientation = orientation;
            return this;
        }
        public SliderPropertiesBuilder SliderType(SlideType sliderType)
        {
            slider.SliderModel.SliderType = sliderType;
            return this;
        }
        //String Values
        public SliderPropertiesBuilder Value(String value)
        {
            slider.SliderModel.Value = value;
            return this;
        }            
        public SliderPropertiesBuilder Height(String height)
        {
            slider.SliderModel.Height = height;
            return this;
        }
        public SliderPropertiesBuilder Width(String width)
        {
            slider.SliderModel.Width = width;
            return this;
        }
        public SliderPropertiesBuilder Values(String values)
        {
            slider.SliderModel.Values = values;
            return this;
        }
        public SliderPropertiesBuilder CssClass(String cssClass)
        {
            slider.SliderModel.CssClass = cssClass;
            return this;
        }
        //Events
        public SliderPropertiesBuilder ClientSideEvents(Action<SliderClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new SliderClientSideEventsBuilder(this.slider.SliderModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }       
        //Render
        public HtmlString Render()
        {
            return new HtmlString(slider.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }

    }
}
