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
using System.Text;
using System.Web.Script.Serialization;
using Syncfusion.JavaScript.Shared.Serializer;
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.DataVisualization.Maps
{
    public class MapUtil
    {
        private string idField = null;
        private List<string> propertyFields = null;

        private ShapeFileDBFReader dbfreader;
        private ShapeFileReader shpreader;

        public MapUtil(string shapepath)
        {
            if (File.Exists(shapepath))
            {
                shpreader = new ShapeFileReader();
                shpreader.ReadShapeFile(shapepath);
            }
        }

        public MapUtil(string shapepath,string dbfPath,string idField)
        {
            if (File.Exists(shapepath))
            {
                shpreader = new ShapeFileReader();
                shpreader.ReadShapeFile(shapepath);

                if (File.Exists(dbfPath))
                {
                    dbfreader = new ShapeFileDBFReader();
                    dbfreader.ReadDBFData(dbfPath);

                    this.idField = idField;
                }
            }
        }

        public MapUtil(string shapepath, string dbfPath, string idField,List<string> propertyFields)
        {
            if (File.Exists(shapepath))
            {
                shpreader = new ShapeFileReader();
                shpreader.ReadShapeFile(shapepath);

                if (File.Exists(dbfPath))
                {
                    dbfreader = new ShapeFileDBFReader();
                    dbfreader.ReadDBFData(dbfPath);

                    this.idField = idField;
                    this.propertyFields = propertyFields;
                }
            }
        }

        public MapUtil(Stream shapeStream)
        {
            if (shapeStream != null)
            {
                shpreader = new ShapeFileReader();
                shpreader.ReadFromStream(shapeStream);
            }
        }

        public MapUtil(Stream shapeStream, Stream dbfStream, string idField)
        {
            if (shapeStream != null)
            {
                shpreader = new ShapeFileReader();
                shpreader.ReadFromStream(shapeStream);

                if (dbfStream != null)
                {
                    dbfreader = new ShapeFileDBFReader();
                    dbfreader.ReadFromStream(dbfStream);

                    this.idField = idField;
                }
            }
        }

        public MapUtil(Stream shapeStream, Stream dbfStream, string idField, List<string> propertyFields)
        {
            if (shapeStream != null)
            {
                shpreader = new ShapeFileReader();
                shpreader.ReadFromStream(shapeStream);

                if (dbfStream != null)
                {
                    dbfreader = new ShapeFileDBFReader();
                    dbfreader.ReadFromStream(dbfStream);

                    this.idField = idField;
                    this.propertyFields = propertyFields;
                }
            }
        }

        public MapUtil(Stream shapeStream, Stream dbfStream, List<string> propertyFields)
        {
            if (shapeStream != null)
            {
                shpreader = new ShapeFileReader();
                shpreader.ReadFromStream(shapeStream);

                if (dbfStream != null)
                {
                    dbfreader = new ShapeFileDBFReader();
                    dbfreader.ReadFromStream(dbfStream);

                    this.propertyFields = propertyFields;
                }
            }
        }

        public object GetGEOJSONData()
        {
            bool isFeature = false;
            int idIndex = 0;
            List<int> propertyIndex = null;

            if (dbfreader != null && dbfreader.DBFData.DBFFields.Count > 0 )
            {
                if (!string.IsNullOrEmpty(this.idField))
                {
                    for (int index = 0; index < dbfreader.DBFData.DBFFields[0].Count; index++)
                    {
                        if (dbfreader.DBFData.DBFFields[0][index].Key.Equals(this.idField))
                        {
                            idIndex = index;
                            isFeature = true;
                            break;
                        }
                    }
                }

                if (this.propertyFields != null)
                {
                    propertyIndex = new List<int>();

                    foreach (string propertyPath in this.propertyFields)
                    {
                        for (int index = 0; index < dbfreader.DBFData.DBFFields[0].Count; index++)
                        {
                            if (dbfreader.DBFData.DBFFields[0][index].Key.Equals(propertyPath))
                            {
                                isFeature = true;
                                propertyIndex.Add(index);
                            }
                        }
                    }
                }
            }

            var jsonobj = new Dictionary<string, object>();

            jsonobj.Add("type", isFeature == true ? "FeatureCollection" : "GeometryCollection");

            object[] container = new object[4];
            container[0] = this.shpreader.Header.MinX;
            container[1] = this.shpreader.Header.MinY;
            container[2] = this.shpreader.Header.MaxX;
            container[3] = this.shpreader.Header.MaxY;
            jsonobj.Add("bbox", container);

            var shapecollection = new object[this.shpreader.Records.Count];
            jsonobj.Add(isFeature == true ? "features" : "geometries", shapecollection);

            for (int i = 0; i < this.shpreader.Records.Count; i++)
            {
                var shape = new Dictionary<string, object>();

                if (isFeature)
                {
                    shape.Add("type", "Feature");

                    object[] box = new object[4];
                    box[0] = this.shpreader.Records[i].MinX;
                    box[1] = this.shpreader.Records[i].MinY;
                    box[2] = this.shpreader.Records[i].MaxX;
                    box[3] = this.shpreader.Records[i].MaxY;

                    shape.Add("bbox", box);

                    if (!string.IsNullOrEmpty(this.idField))
                    {
                        shape.Add("id", dbfreader.DBFData.DBFFields[i][idIndex].Value);
                    }

                    if (propertyIndex != null)
                    {
                        var dbfields = new Dictionary<string, object>();
                        shape.Add("properties", dbfields);

                        foreach (int propIndex in propertyIndex)
                        {
                            dbfields.Add(this.propertyFields[propIndex], dbfreader.DBFData.DBFFields[i][propIndex].Value);
                        }
                    }
                }

                var geofield = new Dictionary<string, object>();

                if (isFeature)
                {
                    shape.Add("geometry", geofield);
                }

                string shapeType = null;

                var cor = this.GetGeometryPoints(i, out shapeType);

                geofield.Add("type", shapeType);

                geofield.Add("coordinates", cor);

                shapecollection[i] = isFeature ? shape : geofield;
            }

            var jss = new JavaScriptSerializer();

            return new MapData(JsonHelper.FormatJson(jss.Serialize(jsonobj)));
        }

        private object[] GetGeometryPoints(int index, out string shapeType)
        {
            shapeType = null;
            if (this.shpreader.Records[index].ShapeType == ShapeType.Polygon)
            {
                ShapeFileRecord record = this.shpreader.Records[index];
                if (record.Parts.Count > 1)
                {
                    shapeType = "MultiPolygon";
                    var pointtypecol = new object[record.Parts.Count];
                    for (int i = 0; i < record.Parts.Count; i++)
                    {
                        object[] pointCol = this.GetPoints(record, i);
                        pointtypecol[i] = new object[] { pointCol };
                    }
                    return pointtypecol;
                }
                else
                {
                    shapeType = "Polygon";
                    object[] pointCol = this.GetPoints(record, 0);
                    return new object[] { pointCol };
                }
            }
            else if (this.shpreader.Records[index].ShapeType == ShapeType.PolyLine)
            {
                ShapeFileRecord record = this.shpreader.Records[index];
                if (record.Parts.Count > 1)
                {
                    shapeType = "MultiPolyLine";
                    var pointtypecol = new object[record.Parts.Count];
                    for (int i = 0; i < record.Parts.Count; i++)
                    {
                        object[] pointCol = this.GetPoints(record, i);
                        pointtypecol[i] = pointCol;
                    }
                    return pointtypecol;
                }
                else
                {
                    shapeType = "PolyLine";
                    object[] pointCol = this.GetPoints(record, 0);
                    return pointCol;
                }
            }
            else if (this.shpreader.Records[index].ShapeType == ShapeType.Point)
            {
                shapeType = "Point";
                object[] points = new object[2];
                points[0] = this.shpreader.Records[index].Points[0].X;
                points[1] = this.shpreader.Records[index].Points[0].Y;
                return points;
            }
            else if (this.shpreader.Records[index].ShapeType == ShapeType.Multipoint)
            {
                shapeType = "Multipoint";
                object[] pointCol = new object[this.shpreader.Records[index].Points.Count];

                for (int j = 0; j < this.shpreader.Records[index].Points.Count; j++)
                {
                    object[] points = new object[2];
                    points[0] = this.shpreader.Records[index].Points[j].X;
                    points[1] = this.shpreader.Records[index].Points[j].Y;
                    pointCol[j] = points;
                }

                return pointCol;
            }
            return null;
        }

        private object[] GetPoints(ShapeFileRecord record, int part)
        {
            int start = record.Parts[part];
            int end;
            
            if (record.Parts.Count > 1 && part != (record.Parts.Count - 1))
            {
                end = record.Parts[part + 1];
            }
            else
            {
                end = record.Points.Count;
            }

            var points_col = (from pta in record.Points.GetRange(start, (end - start))
                              select pta).ToList();

            object[] pointCol = new object[points_col.Count];

            for (int j = 0; j < points_col.Count; j++)
            {
                object[] points = new object[2];
                points[0] = points_col[j].X;
                points[1] = points_col[j].Y;
                pointCol[j] = points;
            }

            return pointCol;
        }
    }

    class JsonHelper
    {
        private const string INDENT_STRING = "    ";

        public static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            return sb.ToString();
        }
    }

    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }
    }

    public class MapData
    {
        internal string jsonString;

        public MapData()
        {
        }

        public MapData(string jsonString)
        {
            this.jsonString = jsonString;
        }

        [JsonProperty("type")]
        [DataMember(Name = "type")]
        public string type { get; set; }

        [JsonProperty("bbox")]
        public object bbox { get; set; }

        [JsonProperty("features")]
        [DataMember(Name = "features")]
        public Feature[] features { get; set; }

        [JsonProperty("geometries")]
        [DataMember(Name = "geometries")]
        public Geometry[] geometries { get; set; }

        public bool ShouldSerializebbox()
        {
            return this.bbox != null;
        }
    }

    public class Feature
    {
        [JsonProperty("type")]
        [DataMember(Name = "type")]
        public string type { get; set; }

        [JsonProperty("geometry")]
        [DataMember(Name = "geometry")]
        public Geometry geometry { get; set; }

        [JsonProperty("properties")]
        [DataMember(Name = "properties")]
        public Dictionary<string, object> properties { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("bbox")]
        [DataMember(Name = "bbox")]
        public object bbox { get; set; }

        [JsonProperty("type")]
        [DataMember(Name = "type")]
        public string type { get; set; }

        [JsonProperty("coordinates")]
        [DataMember(Name = "coordinates")]
        public object coordinates { get; set; }

        public bool ShouldSerializebbox()
        {
            return this.bbox != null;
        }
    }
}
