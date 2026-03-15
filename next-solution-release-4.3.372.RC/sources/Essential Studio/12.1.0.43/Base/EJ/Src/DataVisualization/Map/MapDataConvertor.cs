#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Maps;
using System.Web.Script.Serialization;

namespace Syncfusion.JavaScript.DataVisualization
{
    class MapDataConvertor : DataManagerConverter
    {
        public override string SerializeToJson(object value)
        {
            if (value is MapData)
            {
                MapData mapdata = value as MapData;

                if (!string.IsNullOrEmpty(mapdata.jsonString))
                {
                    return EssentialJavaScript.UnObtrusive ? mapdata.jsonString.Replace("'", "\"") : mapdata.jsonString;
                }
                else
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    ser.MaxJsonLength = int.MaxValue;
                    var str = ser.Serialize(value);
                    return EssentialJavaScript.UnObtrusive ? str.Replace("'", "\"") : str;
                }
            }

            return base.SerializeToJson(value);
        }
    }
}
