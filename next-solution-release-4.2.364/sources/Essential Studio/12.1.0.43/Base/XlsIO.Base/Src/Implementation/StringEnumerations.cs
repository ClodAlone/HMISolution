#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;

namespace Syncfusion.XlsIO
{
    public class StringEnumerations
    {
        #region Private Members

        private const string Solid = "solid";
        private const string Dash = "dash";
        private const string DashDot = "dashDot";
        private const string LongDash = "lgDash";
        private const string SystemDash = "sysDash";
        private const string SystemDot = "sysDot";
        private const string LongDashDot = "lgDashDot";
        private const string LongDashDotDot = "lgDashDotDot";

        private  Dictionary<string, ExcelShapeDashLineStyle> s_dicLineStyleXmlToEnum =
                                           new Dictionary<string, ExcelShapeDashLineStyle>();
        private Dictionary<ExcelShapeDashLineStyle, string> s_dicLineStyleEnumToXml =
                                           new Dictionary<ExcelShapeDashLineStyle, string>();
#endregion

        #region Constructor
        internal StringEnumerations()
        {
            s_dicLineStyleXmlToEnum.Add(Solid, ExcelShapeDashLineStyle.Solid);
            s_dicLineStyleXmlToEnum.Add(Dash, ExcelShapeDashLineStyle.Dashed);
            s_dicLineStyleXmlToEnum.Add(DashDot, ExcelShapeDashLineStyle.Dash_Dot);
            s_dicLineStyleXmlToEnum.Add(LongDash, ExcelShapeDashLineStyle.Medium_Dashed);
            s_dicLineStyleXmlToEnum.Add(SystemDash, ExcelShapeDashLineStyle.Dotted);
            s_dicLineStyleXmlToEnum.Add(SystemDot, ExcelShapeDashLineStyle.Dotted_Round);
            s_dicLineStyleXmlToEnum.Add(LongDashDot, ExcelShapeDashLineStyle.Medium_Dash_Dot);
            s_dicLineStyleXmlToEnum.Add(LongDashDotDot, ExcelShapeDashLineStyle.Dash_Dot_Dot);

            s_dicLineStyleEnumToXml.Add(ExcelShapeDashLineStyle.Solid, Solid);
            s_dicLineStyleEnumToXml.Add(ExcelShapeDashLineStyle.Dashed, Dash);
            s_dicLineStyleEnumToXml.Add(ExcelShapeDashLineStyle.Dash_Dot, DashDot);
            s_dicLineStyleEnumToXml.Add(ExcelShapeDashLineStyle.Dotted, SystemDash);
            s_dicLineStyleEnumToXml.Add(ExcelShapeDashLineStyle.Dotted_Round, SystemDot);
            s_dicLineStyleEnumToXml.Add(ExcelShapeDashLineStyle.Medium_Dashed, LongDash);
            s_dicLineStyleEnumToXml.Add(ExcelShapeDashLineStyle.Medium_Dash_Dot, LongDashDot);
            s_dicLineStyleEnumToXml.Add(ExcelShapeDashLineStyle.Dash_Dot_Dot, LongDashDotDot);
        }
        #endregion

        #region Properties
        internal Dictionary<string, ExcelShapeDashLineStyle> LineDashTypeXmltoEnum
        {
            get
            {
                return s_dicLineStyleXmlToEnum;
            }
        }
        internal Dictionary<ExcelShapeDashLineStyle, string> LineDashTypeEnumToXml
        {
            get
            {
                return s_dicLineStyleEnumToXml;
            }
        }
        #endregion
    }
}