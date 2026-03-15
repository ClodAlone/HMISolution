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
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class RangeNavigatorPropertiesBuilder
    {
         public RangeNavigator rangeNavigator;

         public RangeNavigatorPropertiesBuilder(RangeNavigator rangeNavigator)
          {
              this.rangeNavigator = new RangeNavigator(rangeNavigator.ID, rangeNavigator.RangeNavigatorModel);
          }

         public RangeNavigatorPropertiesBuilder Theme(string theme)
         {
             rangeNavigator.RangeNavigatorModel.Theme = theme;
             return this;
         }

         public RangeNavigatorPropertiesBuilder Padding(string padding)
         {
             rangeNavigator.RangeNavigatorModel.Padding = padding;
             return this;
         }
         public RangeNavigatorPropertiesBuilder ValueType(string valueType)
         {
             rangeNavigator.RangeNavigatorModel.ValueType = valueType;
             return this;
         }
         public RangeNavigatorPropertiesBuilder RangePadding(string padding)
         {
             rangeNavigator.RangeNavigatorModel.RangePadding = padding;
             return this;
         }
         public RangeNavigatorPropertiesBuilder Localization(string localization)
         {
             rangeNavigator.RangeNavigatorModel.Localization = localization;
             return this;
         }
         public RangeNavigatorPropertiesBuilder IsRTL(bool rtl)
         {
             rangeNavigator.RangeNavigatorModel.IsRTL = rtl;
             return this;
         }
         public RangeNavigatorPropertiesBuilder CanResize(bool canResize)
         {
             rangeNavigator.RangeNavigatorModel.CanResize = canResize;
             return this;
         }

         public RangeNavigatorPropertiesBuilder SnappingMode(bool snappingMode)
         {
             rangeNavigator.RangeNavigatorModel.SnappingMode = snappingMode;
             return this;
         }

         public RangeNavigatorPropertiesBuilder IsAsync(bool isasync)
         {
             rangeNavigator.RangeNavigatorModel.IsAsync = isasync;
             return this;
         }

         public RangeNavigatorPropertiesBuilder DeferredUpdate(bool deferredUpdate)
         {
             rangeNavigator.RangeNavigatorModel.DeferredUpdate = deferredUpdate;
             return this;
         }

         public RangeNavigatorPropertiesBuilder AsyncInterval(int asyncInterval)
         {
             rangeNavigator.RangeNavigatorModel.AsyncInterval = asyncInterval;
             return this;
         }
         public RangeNavigatorPropertiesBuilder SelectedData(string selectedData)
         {
             rangeNavigator.RangeNavigatorModel.SelectedData = selectedData;
             return this;
         }
         public RangeNavigatorPropertiesBuilder Loaded(string loaded)
         {
             rangeNavigator.RangeNavigatorModel.Loaded = loaded;
             return this;
         }
         public RangeNavigatorPropertiesBuilder Load(string loaded)
         {
             rangeNavigator.RangeNavigatorModel.Load = loaded;
             return this;
         }
         public RangeNavigatorPropertiesBuilder RangeChanged(string rangeChanged)
         {
             rangeNavigator.RangeNavigatorModel.RangeChanged = rangeChanged;
             return this;
         }
         public RangeNavigatorPropertiesBuilder LabelSettings(Action<LabelSettingBuilder> labelsetting)
         {
             var obj = new LabelSetting ();
             rangeNavigator.RangeNavigatorModel.LabelSetting = obj;
             var builder = new LabelSettingBuilder(obj);
             if (labelsetting != null)
                 labelsetting.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder Size(Action<NavigatorSizeBuilder> labelsetting)
         {
             var obj = new NavigatorSize();
             rangeNavigator.RangeNavigatorModel.Size = obj;
             var builder = new NavigatorSizeBuilder(obj);
             if (labelsetting != null)
                 labelsetting.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder DataSource(Action<NavigatorDataSourceBuilder> ds)
         {
             var obj = new NavigatorDataSource();
             rangeNavigator.RangeNavigatorModel.DataSource = obj;
             var builder = new NavigatorDataSourceBuilder(obj);
             if (ds != null)
                 ds.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder Range(Action<NavigatorRangeBuilder> range)
         {
             var obj = new NavigatorRange();
             rangeNavigator.RangeNavigatorModel.Range = obj;
             var builder = new NavigatorRangeBuilder(obj);
             if (range != null)
                 range.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder SelectedRange(Action<SelectedRangeBuilder> range)
         {
             var obj = new SelectedRange();
             rangeNavigator.RangeNavigatorModel.SelectedRange = obj;
             var builder = new SelectedRangeBuilder(obj);
             if (range != null)
                 range.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder ZoomCordinates(Action<ZoomCordinatesBuilder> range)
         {
             var obj = new ZoomCordinates();
             rangeNavigator.RangeNavigatorModel.ZoomCordinates = obj;
             var builder = new ZoomCordinatesBuilder(obj);
             if (range != null)
                 range.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder Tooltip(Action<TooltipBuilder> range)
         {
             var obj = new Tooltip();
             rangeNavigator.RangeNavigatorModel.Tooltip = obj;
             var builder = new TooltipBuilder(obj);
             if (range != null)
                 range.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder ValueAxisSettings(Action<ValueAxisSettingsBuilder> range)
         {
             var obj = new ValueAxisSettings();
             rangeNavigator.RangeNavigatorModel.ValueAxisSettings = obj;
             var builder = new ValueAxisSettingsBuilder(obj);
             if (range != null)
                 range.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder NavigatorStyle(Action<NavigatorStyleBuilder> range)
         {
             var obj = new NavigatorStyle();
             rangeNavigator.RangeNavigatorModel.NavigatorStyle = obj;
             var builder = new NavigatorStyleBuilder(obj);
             if (range != null)
                 range.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder SeriesSetting(Action<CommonSeriesOptionsBuilder> cs)
         {
             var obj = new CommonSeriesOptions();
             rangeNavigator.RangeNavigatorModel.SeriesSetting = obj;
             var builder = new CommonSeriesOptionsBuilder(obj);
             if (cs != null)
                 cs.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder Series(Action<NavigatorSeriesBuilder> cs)
         {
             var obj = new Series();

             var builder = new NavigatorSeriesBuilder(obj, rangeNavigator);
             if (cs != null)
                 cs.Invoke(builder);
             return this;
         }
         public RangeNavigatorPropertiesBuilder Series(List<Series> series)
         {
             rangeNavigator.RangeNavigatorModel.Series = series;
             return this;
         }
         //Render
         public HtmlString Render()
         {
             return new HtmlString(rangeNavigator.Render().ToString());
         }

         public override String ToString()
         {

             return Render().ToString();
         }
    }
}
