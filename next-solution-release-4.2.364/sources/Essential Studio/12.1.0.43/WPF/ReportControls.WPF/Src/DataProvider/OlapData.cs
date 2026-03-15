//-------------------------------------------------------------------------------------------------
// <copyright file="OlapData.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Reports.DOM;

namespace Syncfusion.Windows.Reports.Data
{
    internal class OlapData
    {
        public OlapData()
        {
            this.DataManager = new OlapDataManager();
            this.Nane = string.Empty;
        }
        public string Nane { get; set; }
        public OlapDataManager DataManager { get; set; }
    }

    internal class CreateReport
    {
        public CreateReport()
        {

        }

        public static OlapReport GetReport(DataSet reportDataSet, string control)
        {
            OlapReport olapReport = new OlapReport();
            DimensionElement[] columnDimension;
            DimensionElement rowDimension = new DimensionElement();
            MeasureElements columnMeasure = new MeasureElements();
            List<string> unicNames = new List<string>();
            List<string> Levels = new List<string>();

            olapReport.CurrentCubeName = UnicNameParse(reportDataSet.Query.CommandText, "FROM [", "]");

            foreach (Field field in reportDataSet.Fields)
            {
                string s = string.Empty;
                if (field.DataField.Contains("xsi:type=\"Level\""))
                {
                    s = UnicNameParse(field.DataField, "UniqueName=\"", "\"");
                    if (s != string.Empty)
                    {
                        unicNames.Add(s);

                        string l = UnicNameParse(s, "[", "]");
                        if (!Levels.Contains(l))
                        {
                            Levels.Add(l);
                        }
                    }
                }
            }

            if (Levels.Count > 1)
            {
                columnDimension = new DimensionElement[(Levels.Count - 1)];

                string l1 = string.Empty, l2 = string.Empty, l3 = string.Empty;
                
                for (int i = 0; i < Levels.Count; i++)
                {
                    List<String> hierarchy = new List<string>();
                    l1 = string.Empty;
                    l2 = string.Empty;
                    l3 = string.Empty;
                    foreach (string name in unicNames)
                    {
                        l1 = UnicNameParse(name, "[", "]");
                        l2 = UnicNameParse(name, l1 + "].[", "]");
                        l3 = UnicNameParse(name, l2 + "].[", "]");

                        if (i < Levels.Count - 1)
                        {
                            if (l1 == Levels[i])
                            {
                                columnDimension[i] = new DimensionElement();
                                columnDimension[i].Name = l1;
                                columnDimension[i].AddLevel(l2, l3);
                                //hierarchy.Add("[" + l2 + "." + l3 + "]");
                            }
                        }
                        else
                        {
                            if (l1 == Levels[i])
                            {
                                rowDimension.Name = l1;
                                rowDimension.AddLevel(l2, l3);
                                //hierarchy.Add("[" + l2 + "." + l3 + "]");
                            }
                        }
                    }
                }

                for (int i = 0; i < columnDimension.Count(); i++)
                {
                    olapReport.CategoricalElements.Add(new Item { ElementValue = columnDimension[i] });
                }
                olapReport.SeriesElements.Add(new Item { ElementValue = rowDimension });
            }
            else if (Levels.Count == 1)
            {
                columnDimension = new DimensionElement[1];
                string l1 = string.Empty, l2 = string.Empty, l3 = string.Empty;
                foreach (string name in unicNames)
                {
                    l1 = UnicNameParse(name, "[", "]");
                    l2 = UnicNameParse(name, l1 + "].[", "]");
                    l3 = UnicNameParse(name, l2 + "].[", "]");

                    if (l1 == Levels[0])
                    {
                        columnDimension[0] = new DimensionElement();
                        columnDimension[0].Name = l1;
                        columnDimension[0].AddLevel(l2, l3);
                        olapReport.CategoricalElements.Add(new Item { ElementValue = columnDimension[0] });
                        //hierarchy.Add("[" + l2 + "." + l3 + "]");
                    }
                }
            }


            Regex reg = new Regex("(\\[Measures\\]\\.\\[)[\\w]+([\\s][\\w]+)*(\\])");
            MatchCollection measures = reg.Matches(reportDataSet.Query.CommandText);

            foreach (Match element in measures)
            {
                string measureName = UnicNameParse(element.Value, ".[", "]");
                columnMeasure.Elements.Add(new MeasureElement { Name = measureName });
            }

            
            if (control == "Gauge" ||(Levels.Count ==0 && (control == "Tablix"||control == "Chart")))
            {
                KpiElements kpiElements = new KpiElements();

                Regex re = new Regex("(((KPI)((Status)|(Value)|(Trend)|(Goal)))\\(\")[\\w]+([\\s][\\w]+)*(\"\\))");
                MatchCollection kpis = re.Matches(reportDataSet.Query.CommandText);
                int s1 = kpis.Count;

                List<string> elements = new List<string>();
                foreach (Match element in kpis)
                {
                    string kpiName = UnicNameParse(element.Value, "\"", "\"");
                    if (!elements.Contains(kpiName))
                    {
                        elements.Add(kpiName);
                        kpiElements.Elements.Add(new KpiElement { Name = kpiName, ShowKPIGoal = true, ShowKPIStatus = true, ShowKPIValue = true, ShowKPITrend = true });
                    }
                }
                if (kpiElements.Elements.Count > 0)
                    olapReport.CategoricalElements.Add(new Item { ElementValue = kpiElements });
            }

            if (columnMeasure.Elements.Count > 0)
            {
                olapReport.CategoricalElements.Add(new Item { ElementValue = columnMeasure });
            }
            
            return olapReport;
        }

        private static string UnicNameParse(string field, string startsWith, string endsWith)
        {
            int start = field.IndexOf(startsWith) + startsWith.Length;
            int end = field.IndexOf(endsWith, start);
            if ((end - start) > 0)
            {
                return field.Substring(start, (end - start));
            }
            else
                return string.Empty;
        }
    }
}
