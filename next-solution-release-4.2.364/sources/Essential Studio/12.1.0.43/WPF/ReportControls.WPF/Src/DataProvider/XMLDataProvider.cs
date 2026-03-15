//-------------------------------------------------------------------------------------------------
// <copyright file="SqlDataProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.RDL.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Xml.Linq;
    using System.IO;
    using System.Xml;
    using System.Text;

    /// <summary>
    /// A XML data provider for EmbededXML datasource.
    /// </summary>
    internal class XMLDataProvider
    {
        private ReportModel reportModel;

        private Dictionary<object, object> XmlRefValue = null;

        public XMLDataProvider()
        {
        }

        public XMLDataProvider(ReportModel model)
        {
            this.reportModel = model;
        }

        public List<ReportData> GetData(string connectionString, string command)
        {
            var list = new List<ReportData>();
            var table = GetTable(connectionString, command, "Table");

            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    ReportData data = new ReportData();
                    data.Data = new Dictionary<string, object>();

                    foreach (DataColumn columninfo in table.Columns)
                    {
                        object value = row[columninfo.ColumnName];

                        if (value == System.DBNull.Value)
                        {
                            value = null;
                        }

                        data.Data.Add(columninfo.ColumnName, value);
                    }

                    list.Add(data);
                }
            }

            return list;
        }

        public DataTable GetTable(string connectionString, string command, string tableName)
        {
            DataSet ds = new DataSet();
            string elementPath = string.Empty;
            XElement data = null;

            if (!string.IsNullOrEmpty(connectionString))
            {
                string xmlPath = connectionString;

                if (this.reportModel != null)
                {
                    string reportPath = this.reportModel.ReportPath;

                    if (!System.IO.Path.IsPathRooted(xmlPath) && !string.IsNullOrEmpty(reportPath) && !(xmlPath.Contains("http")))
                    {
                        xmlPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(reportPath), xmlPath);
                    }
                }

                XmlReader reader = System.Xml.XmlReader.Create(xmlPath);
                data = XElement.Load(reader);

                if (!string.IsNullOrEmpty(command))
                {
                    elementPath = command;

                    if (command.Contains("ElementPath"))
                    {
                        XmlReader queryReader = XmlReader.Create(new System.IO.StringReader(command));
                        XElement xQuery = XElement.Load(queryReader);
                        XElement x_elementPath = xQuery.Elements("ElementPath").FirstOrDefault();
                        elementPath = x_elementPath.Value;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(command))
            {
                System.Xml.XmlReader stream = System.Xml.XmlReader.Create(new System.IO.StringReader(command));
                XElement xQuery = XElement.Load(stream);
                data = xQuery.Elements("XmlData").Elements().FirstOrDefault();
                IEnumerable<XElement> x_elementPath = xQuery.Elements("ElementPath");

                if (x_elementPath != null && x_elementPath.Count() > 0)
                {
                    elementPath = x_elementPath.FirstOrDefault().Value;
                }
            }

            if (data != null)
            {
                if (!string.IsNullOrEmpty(elementPath))
                {
                    if (ValidateQuery(elementPath))
                    {
                        ProcessElementPath(data, elementPath);
                    }
                }
                StringReader sr = new StringReader(data.ToString());
                ds.ReadXml(sr);
                DataTable dataTable = this.CreateJoinTable(ds);
                dataTable.TableName = tableName;
                return dataTable;
            }

            throw new Exception("Datasource does not support");
        }

        DataTable CreateJoinTable(DataSet set)
        {
            if (set.Tables.Count == 1 && this.XmlRefValue == null)
            {
                return set.Tables[0];
            }

            DataTable dataSourceTable = new DataTable();
            DataTable partable = set.Tables[set.Tables.Count - 1];
            List<TableColumn> columnCollection = new List<TableColumn>();
            List<string> relations = new List<string>();
            int level = -1;

            for (int i = set.Tables.Count - 1; i >= 0; i--)
            {
                DataTable sourceTable = set.Tables[i];
                level++;

                foreach (System.Data.DataColumn column in sourceTable.Columns)
                {
                    bool columnAdded = true;

                    if (sourceTable.ParentRelations != null && sourceTable.ParentRelations.Count > 0)
                    {
                        var collumnDetails = from col in sourceTable.ParentRelations[0].ChildColumns where col.ColumnName == column.ColumnName select col;
                        columnAdded = collumnDetails.Count() == 0;

                        if (collumnDetails.Count() > 0)
                        {
                            relations.Add(sourceTable.ParentRelations[0].RelationName);
                        }
                    }

                    if (columnAdded && !dataSourceTable.Columns.Contains(column.ColumnName))
                    {
                        columnCollection.Add(new TableColumn() { ColumnName = column.ColumnName, TableLevel = level });
                        if (column.ColumnName.EndsWith("_Text"))
                        {
                            dataSourceTable.Columns.Add(column.ColumnName.Replace("_Text", ""), column.DataType, column.Expression);
                        }
                        else
                        {
                            dataSourceTable.Columns.Add(column.ColumnName, column.DataType, column.Expression);
                        }
                    }
                }
            }

            foreach (DataRow row in partable.Rows)
            {
                List<object> datas = new List<object>();
                try
                {
                    foreach (var column in columnCollection)
                    {
                        DataRow dataRow = row;

                        for (int i = 0; i < column.TableLevel; i++)
                        {
                            dataRow = dataRow.GetParentRow(relations[i]);
                        }

                        var dtrow = dataRow[column.ColumnName];

                        if (this.XmlRefValue != null && this.XmlRefValue.ContainsKey(dtrow))
                        {
                            datas.Add(this.XmlRefValue[dtrow]);
                        }
                        else
                        {
                            datas.Add(dtrow);
                        }

                    }

                    dataSourceTable.Rows.Add(datas.ToArray());
                }
                catch { }
            }

            return dataSourceTable;
        }

        public bool ValidateQuery(string elementPath)
        {
            string pathPattern = @"^(\s*[0-9a-zA-Z]*\s*({(\s*(,\s*)?[\@]\s*[0-9a-zA-Z]*\s*)*}\s*)?(/\s*[0-9a-zA-Z]+\s*({(\s*(,)?[\@][0-9a-zA-Z]*\s*)*}\s*)?)*)$";
            if (System.Text.RegularExpressions.Regex.IsMatch(elementPath, pathPattern))
            {
                string temp = elementPath.Replace('{', '$');
                temp = temp.Replace('}', '$');
                string[] references = temp.Split('$').ToArray();
                references = (from str in references where str.Contains("@") select str).ToArray();
                int count = 0;
                foreach (var str in references)
                {
                    count += str.Split(',').Select(s => s.Trim()).Where(s => s.Length == 1).Count();
                    if (count > 1)
                        throw new Exception("The XmlDP query contains more than one reference to the attribute/element @.");
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void ProcessElementPath(XElement xmldata, string elementPath)
        {
            string[] subPaths = elementPath.Split('/');
            string modifiedPath = elementPath.Replace(subPaths[0] + "/", "");
            IEnumerable<XElement> elementList = xmldata.Elements();

            if (xmldata != null && subPaths != null)
            {
                foreach (var element in elementList.ToList())
                {
                    if (element.HasElements)
                    {
                        ProcessElementPath(element, modifiedPath);
                    }
                }
            }

            subPaths = subPaths.Select(str => str.Trim()).ToArray();
            int index = subPaths[0].IndexOf('{');
            int length = subPaths[0].IndexOf('}') - (index + 1);
            string references = null;

            if (length > -1)
            {
                references = subPaths[0].Substring(index + 1, length);
                modifiedPath = subPaths[0].Substring(0, index);

                for (int i = 0; i < subPaths.Count(); i++)
                {
                    if (subPaths[i].Equals(subPaths[0]))
                    {
                        subPaths[i] = modifiedPath;
                    }
                }
            }

            string[] attributes = references != null ? references.Split(',') : null;

            if (attributes != null && subPaths.Contains(xmldata.Name.LocalName))
            {
                XElement currentElement = XElement.Parse(xmldata.ToString());
                string current = xmldata.Name.LocalName;
                bool removeChild = (from node in xmldata.Elements()
                                    where subPaths.Contains(node.Name.LocalName)
                                    select node).Count() == 0;

                if (xmldata.HasElements && removeChild)
                {
                    xmldata.Nodes().Remove();
                }
                if (attributes.Count() == 1 && string.IsNullOrEmpty(attributes[0]))
                {
                    if (removeChild)
                        xmldata.Remove();
                    else
                        xmldata.RemoveAttributes();
                }
                else
                {
                    if (attributes.Contains("@"))
                    {
                        StringBuilder value = new StringBuilder();
                        if (XmlRefValue == null)
                        {
                            XmlRefValue = new Dictionary<object, object>();
                        }
                        foreach (var node in currentElement.Elements())
                        {
                            value.Append(node.ToString());
                        }

                        string objectID = Guid.NewGuid().ToString().Split('-')[0];
                        XmlRefValue.Add(objectID, value);
                        xmldata.SetValue(objectID);
                        attributes = (from str in attributes where !str.Equals("@") select str).ToArray();
                    }
                    if (attributes.Count() != 0)
                    {
                        attributes = (from str in attributes select str.Replace("@", "")).ToArray();

                        foreach (var attrib in attributes)
                        {
                            if (xmldata.Attribute(attrib) == null)
                            {
                                xmldata.Add(new XAttribute(attrib, ""));
                            }
                            else
                            {
                                xmldata.Attribute(attrib).Remove();
                            }
                        }
                        var ignoreattrib = from attrib in xmldata.Attributes()
                                           where !attributes.Contains(attrib.Name.LocalName)
                                           select attrib.Name;
                        foreach (var attrib in ignoreattrib.ToList())
                        {
                            xmldata.Attribute(attrib).Remove();
                        }
                    }
                }
            }
            else if (!subPaths.Contains(xmldata.Name.LocalName))
            {
                xmldata.Remove();
            }
        }
    }

    class TableColumn
    {
        public string ColumnName { get; set; }
        public int TableLevel { get; set; }
    }
}
