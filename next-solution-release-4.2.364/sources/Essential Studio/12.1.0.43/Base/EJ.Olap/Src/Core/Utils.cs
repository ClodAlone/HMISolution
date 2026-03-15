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
using Syncfusion.Olap.Reports;
using System.IO;
using System.Xml.Serialization;
using System.IO.Compression;
using System.Web;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Data;
using System.Globalization;

namespace Syncfusion.JavaScript.Olap
{
    public static class Utils
    {
        public static string SerializeOlapReport(OlapReport report)
        {
            if (report != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    new XmlSerializer(typeof(OlapReport)).Serialize(stream, report);
                    var data = stream.ToArray();
                    return CompressData(data);
                }
            }

            return "";
        }

        public static OlapReport DeserializeOlapReport(string report)
        {
            if (string.IsNullOrEmpty(report)) return null;

            using (var stream = new MemoryStream(DecompressData(report)))
            {
                return new XmlSerializer(typeof(OlapReport)).Deserialize(stream) as OlapReport;
            }
        }

        public static string Compress(this string data)
        {
            return CompressData(data.Select(d => (byte)d).ToArray());
        }

        public static string Decompress(this string data)
        {
            return new string(DecompressData(data).Select(s => (char)s).ToArray());
        }

        public static string CompressData(byte[] data)
        {
            using (MemoryStream strCompress = new MemoryStream())
            {
                GZipStream sw = new GZipStream(strCompress, CompressionMode.Compress);
                sw.Write(data, 0, data.Length);
                sw.Close();

                return Convert.ToBase64String(strCompress.ToArray());
            }
        }

        public static byte[] DecompressData(string cString)
        {
            var cData = Convert.FromBase64String(cString);

            using (MemoryStream strDecompress = new MemoryStream(cData))
            {
                GZipStream sr = new GZipStream(strDecompress, CompressionMode.Decompress);

                using (MemoryStream ms = new MemoryStream())
                {
                    for (int b = sr.ReadByte(); b != -1; b = sr.ReadByte())
                        ms.WriteByte((byte)b);

                    sr.Close();
                    return ms.ToArray();
                }
            }
        }

        public static void AppendGZip(this HttpContextBase context)
        {
            string acceptEncoding = context.Request.Headers["Accept-Encoding"];
            Stream prevUncompressedStream = context.Response.Filter;

            if (acceptEncoding != null && acceptEncoding.Length != 0)
            {
                acceptEncoding = acceptEncoding.ToLower();

                if (acceptEncoding.Contains("deflate") || acceptEncoding == "*")
                {
                    context.Response.Filter = new DeflateStream(prevUncompressedStream, CompressionMode.Compress);
                    context.Response.AppendHeader("Content-Encoding", "deflate");
                }
                else if (acceptEncoding.Contains("gzip"))
                {
                    context.Response.Filter = new GZipStream(prevUncompressedStream, CompressionMode.Compress);
                    context.Response.AppendHeader("Content-Encoding", "gzip");
                }
            }
        }

        public static string GetDrillData(PivotCellDescriptor cellDescriptor, List<PositionInfo> infos)
        {
            var member = cellDescriptor.Tag as Member;
            bool drillPosition = infos != null && infos.Count > 0;

            return member != null
                ? string.Format(CultureInfo.CurrentCulture, drillPosition ? "{0}::{1}::{2}::{3}::{4}::{5}::{6}::{7}::{8}" : "{0}::{1}::{2}::{3}::{4}::{5}",
                    member.UniqueName, member.LevelUniqueName, member.Caption, member.ParentUniqueName, member.ParentCaption,
                    ((int)cellDescriptor.CellType).ToString(),
                    drillPosition ? new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(infos) : "",
                    drillPosition ? member.ParentHierarchy : "",
                    drillPosition ? ((int)cellDescriptor.ExpandableState).ToString() : "") : "";
        }

        public static MemoryStream GetReportStream(string clientReports)
        {
            string reportAsXML = clientReports.Decompress();
            reportAsXML = reportAsXML.Remove(0, reportAsXML.IndexOf('<'));
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(reportAsXML);
            MemoryStream reportStream = new MemoryStream();
            reportStream.Write(buffer, 0, buffer.Length);
            return reportStream;
        }
    }
}
