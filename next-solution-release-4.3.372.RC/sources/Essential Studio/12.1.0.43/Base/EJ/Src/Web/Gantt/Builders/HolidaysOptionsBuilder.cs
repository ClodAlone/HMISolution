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
    public class HolidaysOptionsBuilder
    {
        private HolidaysOptions holidaysOptions = new HolidaysOptions();

        public HolidaysOptionsBuilder(HolidaysOptions holidays)
        {
            holidaysOptions = holidays;
        }

        public HolidaysOptionsBuilder Holidays(Action<HolidaysBuilder> holidays)
        {
            var builder = new HolidaysBuilder(holidaysOptions);
            if (builder != null)
                holidays.Invoke(builder);
            return this;
        }

        public HolidaysOptionsBuilder StripLines(List<Holidays> holidays)
        {
            holidaysOptions.Holidays = holidays;
            return this;
        }
    }
}