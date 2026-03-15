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
using System.Web;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class HolidaysBuilder
    {
        private Holidays holidays;
        public HolidaysOptions holidaysOptions = new HolidaysOptions();

        public HolidaysBuilder(HolidaysOptions holidaysOptions)
        {
            this.holidaysOptions = holidaysOptions;
            holidays = new Holidays();
        }

        public HolidaysBuilder Day(String day)
        {
            holidays.Day = day;
            return this;
        }

        public HolidaysBuilder Label(String label)
        {
            holidays.Label = label;
            return this;
        }

        public HolidaysBuilder Background(String background)
        {
            holidays.Background = background;
            return this;
        }


        public void Add()
        {
            this.holidaysOptions.Holidays.Add(holidays);
            holidays = new Holidays();
            //return this; 
        }
    }
}