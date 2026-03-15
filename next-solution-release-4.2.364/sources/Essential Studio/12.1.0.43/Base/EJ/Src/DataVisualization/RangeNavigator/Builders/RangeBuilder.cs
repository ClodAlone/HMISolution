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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization 
{
   public class NavigatorRangeBuilder
    {
        private NavigatorRange m_Range;
        public NavigatorRangeBuilder(NavigatorRange range)
        {
            this.m_Range = range;
        }



        public NavigatorRangeBuilder Start(string start)
        {
            this.m_Range.Start = start;
            return this;
        }

        public NavigatorRangeBuilder End(string end)
        {
            this.m_Range.End = end;
            return this;
        }

         
    }
   public class SelectedRangeBuilder
   {
       private SelectedRange m_Range;
       public SelectedRangeBuilder(SelectedRange range)
       {
           this.m_Range = range;
       }



       public SelectedRangeBuilder Start(string start)
       {
           this.m_Range.Start = start;
           return this;
       }

       public SelectedRangeBuilder End(string end)
       {
           this.m_Range.End = end;
           return this;
       }


   }
   public class ZoomCordinatesBuilder
   {
       private ZoomCordinates zoomCordinates;
       public ZoomCordinatesBuilder(ZoomCordinates range)
       {
           this.zoomCordinates = range;
       }



       public ZoomCordinatesBuilder ZoomFactor(string start)
       {
           this.zoomCordinates.ZoomFactor = start;
           return this;
       }

       public ZoomCordinatesBuilder ZoomPosition(string end)
       {
           this.zoomCordinates.ZoomPosition = end;
           return this;
       }


   }
}
