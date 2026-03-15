#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Syncfusion.Windows.Reports.DOM;
using Syncfusion.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
#if !SILVERLIGHT
using Microsoft.VisualBasic;
using System.CodeDom.Compiler;
using System.Data;
#endif
using System.IO;
using System.ComponentModel;
using System.Globalization;
using Syncfusion.Linq;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Reports.Controls.Data;
using System.Security.Principal;
using Syncfusion.Reports.Controls.Utils;

namespace Syncfusion.Windows.Reports.Controls.Internal
{
    class ExpressionEngine
    {
        private ReportModel pageModel = null;

        private Dictionary<string, object> classCollection;

        internal Dictionary<string, object> FieldValues { get; set; }

        internal Dictionary<string, object> RowNumbers { get; set; }

        internal ExpressionEngine()
        {
        }

        internal ExpressionEngine(ReportModel pagemodel)
        {
            this.FieldValues = new Dictionary<string, object>();
            this.RowNumbers = new Dictionary<string, object>();
            this.pageModel = pagemodel;

            this.classCollection = new Dictionary<string, object>();
            GenerateClasses();
        }

        internal void GenerateClasses()
        {
            Assembly codeAssembly = null;

#if !SILVERLIGHT
            if (this.pageModel.Report.Code != null)
            {
                VBCodeProvider provider = new VBCodeProvider();
                CompilerParameters cp = new CompilerParameters();

                cp.ReferencedAssemblies.Add("system.dll");
                cp.ReferencedAssemblies.Add("system.data.dll");
                cp.ReferencedAssemblies.Add("system.xml.dll");
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;
                cp.IncludeDebugInformation = false;

                string[] codes = this.pageModel.Report.Code.Split('\n'); ;

                List<string> vbCodes = new List<string>();
                vbCodes.Add("Imports System");
                vbCodes.Add("Imports Microsoft.VisualBasic");
                vbCodes.Add("Imports System.Convert");
                vbCodes.Add("Imports System.Math");
                vbCodes.Add("namespace Syncfusion.Reports.Controls.Data");
                vbCodes.Add("Public Class Code");

                foreach (var vbcode in codes)
                {
                    vbCodes.Add(vbcode);
                }

                vbCodes.Add("End Class");
                vbCodes.Add("End Namespace");

                StringBuilder code = new StringBuilder();

                foreach (var line in vbCodes)
                {
                    code.Append(line);
                    code.Append("\r\n");
                }

                CompilerResults cr = provider.CompileAssemblyFromSource(cp, code.ToString());
                {
                    if (cr.Errors.HasErrors)
                    {
                        string errorText = String.Empty;
                        for (int i = 0; i < cr.Output.Count; i++)
                            errorText += cr.Output[i] + '\n';
                        for (int i = 0; i < cr.Errors.Count; i++)
                            errorText += i.ToString() + ": " + cr.Errors[i].ToString() + '\n';

                        throw new Exception(errorText);
                    }

                    codeAssembly = cr.CompiledAssembly;
                }
            }
#endif
            UpdateClass(codeAssembly);

            if (this.pageModel.Report.CodeModules != null)
            {
                foreach (var codeModule in this.pageModel.Report.CodeModules)
                {
                    codeAssembly = null;
#if SILVERLIGHT
                    string assName = codeModule.Value.Split(',')[0].Trim();

                    foreach (var part in Deployment.Current.Parts)
                    {
                        if (part.Source.Contains(assName))
                        {
                            System.Windows.Resources.StreamResourceInfo info = Application.GetResourceStream(new Uri(part.Source, UriKind.Relative));
                            codeAssembly = new AssemblyPart().Load(info.Stream);
                            break;
                        }
                    }
#else
                    codeAssembly = Assembly.Load(codeModule.Value);
#endif
                    UpdateClass(codeAssembly);
                }
            }
        }

        public void UpdateClass(Assembly assembly)
        {
            if (assembly != null)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass)
                    {
                        try
                        {
                            object obj = assembly.CreateInstance(type.FullName);
                            this.classCollection.Add(type.Name, obj);
                            this.classCollection.Add(type.FullName, obj);
                        }
                        catch
                        {
                            this.classCollection.Add(type.Name, type);
                            this.classCollection.Add(type.FullName, type);
                        }
                    }

                }
            }
        }

        internal string EvalValue(string function, object[] parameters)
        {
            string[] classMethods = function.Split('.');

            object obj = null;
            System.Type objectType = null;

            if (this.classCollection.ContainsKey(classMethods[0]))
            {
                if (this.classCollection[classMethods[0]] is System.Type)
                {
                    obj = null;
                    objectType = this.classCollection[classMethods[0]] as System.Type;
                }
                else
                {
                    obj = this.classCollection[classMethods[0]];
                    objectType = obj.GetType();
                }
            }

            for (int i = 1; i < classMethods.Length; i++)
            {
                if (i == classMethods.Length - 1 && objectType != null)
                {
                    string methodName = classMethods[i];
                    var methodInfo = (from method in objectType.GetMethods()
                                      where method.Name.Equals(methodName) && method.GetParameters().Count() == parameters.Length
                                      select method).First();

                    ParameterInfo[] paramsInfo = methodInfo.GetParameters();
                    object[] paramValues = new object[paramsInfo.Length];
                    int paramCount = 0;

                    foreach (var parameterInfo in methodInfo.GetParameters())
                    {
                        object value = null;

                        if (paramCount < parameters.Length)
                        {
                            string parameterType = parameterInfo.ParameterType.Name.ToLower();
                            if (parameterType.Contains("decimal"))
                            {
                                value = Convert.ToDecimal(parameters[paramCount].ToString());
                            }
                            else if (parameterType.Contains("double"))
                            {
                                value = double.Parse(parameters[paramCount].ToString());
                            }
                            else if (parameterType.Contains("int16"))
                            {
                                value = Convert.ToInt16(double.Parse(parameters[paramCount].ToString()));
                            }
                            else if (parameterType.Contains("int32"))
                            {
                                value = Convert.ToInt32(double.Parse(parameters[paramCount].ToString()));
                            }
                            else if (parameterType.Contains("int64"))
                            {
                                value = Convert.ToInt64(double.Parse(parameters[paramCount].ToString()));
                            }
                            else if (parameterType.Contains("DateTime"))
                            {
                                value = Convert.ToDateTime(parameters[paramCount].ToString());
                            }
                            else if (parameterType.ToLower().Contains("string"))
                            {
                                value = parameters[paramCount] != null ? parameters[paramCount].ToString() : null;
                            }
                        }

                        paramValues[paramCount] = value;
                    }

                    string result = methodInfo.Invoke(obj, paramValues).ToString();
                    return result;
                }
                else
                {
                    string name = classMethods[i];

                    if (objectType == null)
                    {
                        name = classMethods[0];
                        i = 0;

                        while (i < classMethods.Length && !this.classCollection.ContainsKey(name))
                        {
                            i++;
                            name += "." + classMethods[i];
                        }

                        if (i < classMethods.Length)
                        {
                            if (this.classCollection[name] is System.Type)
                            {
                                obj = null;
                                objectType = this.classCollection[name] as System.Type;
                            }
                            else
                            {
                                obj = this.classCollection[name];
                                objectType = obj.GetType();
                            }
                        }
                        else
                        {
                            return "#Error";
                        }
                    }
                    else
                    {
                        obj = objectType.GetProperty(name).GetValue(obj, null);
                        objectType = obj.GetType();
                    }
                }
            }

            return "#Error";
        }

        public string GetExpressionStringValue(string expression)
        {
            return this.GetExpressionStringValue(expression, false);
        }

        public string GetExpressionStringValue(string expression, bool isDataSetExec)
        {
            if (expression != null)
            {
                object value = this.GetExpressionValue(expression, isDataSetExec);

                if (value != null)
                {
                    return value.ToString();
                }
            }

            return null;
        }

        public object GetExpressionValue(string expression)
        {
            return this.GetExpressionValue(expression, false);
        }

        public object GetExpressionValue(string expression, bool isDataSetExec)
        {
            if (expression.Trim().StartsWith("="))
            {
                expression = expression.Trim().Substring(1);
            }
            else
            {
                return expression;
            }

            expression = expression.Replace("&gt;", ">");
            expression = expression.Replace("&lt;", "<");
            if (expression.Contains("User!Language"))
                expression = expression.Replace("User!Language", "\"" + CultureInfo.CurrentCulture.Name + "\"");
            if (expression.Contains("Globals!TotalPages"))
                expression = expression.Replace("Globals!TotalPages", "\"" + "Globals.TotalPages" + "\"");
            if (expression.Contains("Globals!OverallTotalPages"))
                expression = expression.Replace("Globals!OverallTotalPages", "\"" + "Globals.TotalPages" + "\"");
            if (expression.Contains("Globals.PageNumber"))
                expression = expression.Replace("Globals.PageNumber", "\"" + "Globals.PageNumber" + "\"");
            if (expression.Contains("Globals!PageNumber"))
                expression = expression.Replace("Globals!PageNumber", "\"" + "Globals.PageNumber" + "\"");

#if !SILVERLIGHT
            if (expression.Contains("User!UserID"))
                expression = expression.Replace("User!UserID", "\"" + WindowsIdentity.GetCurrent().Name + "\"");
#endif

            expression = expression.Replace("<>", "!=");
            string evaluatedExp = null;
            UpdateExpressionValue(expression, null, out evaluatedExp, true, isDataSetExec);
            ExpressionEval eval = new ExpressionEval(evaluatedExp, this);
            object evaluatedValue = eval.Evaluate();
            return evaluatedValue;
        }


        public List<string> GetExpressionFields(string expression, string datasetName, out string resultExpression)
        {
            List<string> expressionFields = new List<string>();
            resultExpression = expression;
            Match match = null;

            do
            {
                match = new Regex(@"Fields!\s*(?<Field>\w+)\s*.Value").Match(expression);

                if (match.Success)
                {
                    string filedExpression = match.Value;
                    string field = this.GetFieldName(match.Groups["Field"].Value, this.pageModel.Report, datasetName);
                    expression = expression.Replace(filedExpression, "[" + field + "]");
                    resultExpression = resultExpression.Replace(filedExpression, "Fields!" + field + ".Value");

                    if (!expressionFields.Contains(field))
                    {
                        expressionFields.Add(field);
                    }
                }
            } while (match.Success);

            return expressionFields;
        }

        public string UpdateExpressionDataSet(string expression, string dataSetname)
        {
            string resultExpression = expression;
            Match match = null;

            do
            {
                match = new Regex(@"(?<Data>(?<Function>\w+)\s*\(Fields!\s*(?<Field>\w+).Value,\s*\" + "\"" + @"(?<DataSet>\w+)\" + "\"" + @"\))").Match(expression);
                if (match.Success)
                {
                    expression = expression.Replace(match.Value, String.Empty);
                }
            } while (match.Success);

            do
            {
                match = new Regex(@"(?<Data>(?<Function>\w+)\s*\(Fields!\s*(?<Field>\w+)\s*.Value\s*\))").Match(expression);

                if (match.Success)
                {
                    expression = expression.Replace(match.Value, String.Empty);
                    string fieldExpression = match.Groups["Function"].Value + "(Fields!" + match.Groups["Field"].Value + ".Value,\"" + dataSetname + "\")";
                    resultExpression = resultExpression.Replace(match.Value, fieldExpression);
                }
            } while (match.Success);

            do
            {
                match = new Regex(@"Fields!\s*(?<Field>\w+)\s*.Value").Match(expression);

                if (match.Success)
                {
                    expression = expression.Replace(match.Value, String.Empty);
                    string fieldExpression = "First(Fields!" + match.Groups["Field"].Value + ".Value,\"" + dataSetname + "\")";
                    resultExpression = resultExpression.Replace(match.Value, fieldExpression);
                }
            } while (match.Success);

            return resultExpression;
        }

        private string[] GetParameters(Match m, string expression)
        {
            string strParameters = "";
            int index = m.Index + m.Length;
            int level = 1;
            int lastIndex = 0;
            bool isInQuotes = false;
            List<string> retExprs = new List<string>();

            //Get the parameter string
            while (level > 0)
            {
                if (index >= expression.Length)
                    throw new ArgumentException("Missing ')' in Expression");

                if (!isInQuotes && expression[index] == ')')
                {
                    level--;
                }
                if (!isInQuotes && expression[index] == '(')
                {
                    level++;
                }

                if (expression[index] == '"' && (index == 0 || expression[index - 1] != '\\'))
                {
                    isInQuotes = !isInQuotes;
                }

                if (level > 0)
                {
                    index++;
                }
            }

            strParameters = expression.Substring(m.Index + m.Length, index - (m.Index + m.Length));

            if ("" + strParameters == "")
                return null;

            isInQuotes = false;
            for (index = 0; index < strParameters.Length; index++)
            {
                if (!isInQuotes && strParameters[index] == ')')
                {
                    level--;
                }
                if (!isInQuotes && strParameters[index] == '(')
                {
                    level++;
                }

                if (strParameters[index] == '"' && (index == 0 || strParameters[index - 1] != '\\'))
                {
                    isInQuotes = !isInQuotes;
                }

                if (!isInQuotes && level == 0 && strParameters[index] == ',')
                {
                    retExprs.Add(strParameters.Substring(lastIndex, index - lastIndex));
                    lastIndex = index + 1;
                }
            }

            retExprs.Add(strParameters.Substring(lastIndex, index - lastIndex));

            return retExprs.ToArray();
        }

        public List<DataField> GetExpressionDataFields(string expression, string dataSetname, out string resultExpression)
        {
            return UpdateExpressionValue(expression, dataSetname, out resultExpression, false,false);
        }

        public List<DataField> UpdateExpressionValue(string expression, string dataSetname, out string resultExpression, bool executeExpression,bool isDataSetExec)
        {
            List<DataField> expressionFields = new List<DataField>();
            resultExpression = expression;
            Match match = null;

            do
            {
                match = ExpressionMatch.Function.Match(expression);

                if (match.Success)
                {
                    if (this.IsAggregateFunction(match.Groups["Function"].Value))
                    {
                        int totalIndex = match.Index + match.Length;
                        int level = 1;
                        bool isInQuotes = false;

                        while (level > 0)
                        {
                            if (totalIndex >= expression.Length)
                            {
                                throw new Exception("Invalid Expression :" + match);
                            }
                            if (!isInQuotes && expression[totalIndex] == ')')
                            {
                                level--;
                            }
                            if (!isInQuotes && expression[totalIndex] == '(')
                            {
                                level++;
                            }

                            if (expression[totalIndex] == '"' && (totalIndex == 0 || expression[totalIndex - 1] != '\\'))
                            {
                                isInQuotes = !isInQuotes;
                            }

                            totalIndex++;
                        }

                        string filedExpression = expression.Substring(match.Index, (totalIndex) - match.Index);

                        string[] parameters = this.GetParameters(match, expression);

                        if (parameters.Length == 1 && !executeExpression)
                        {
                            string subexp = parameters[0].Trim();
                            Match subMatch = new Regex(@"^Fields!\s*(?<Field>\w+)\s*.Value$").Match(subexp);

                            if (subMatch.Success)
                            {
                                string subEvaluatedExpression = null;
                                var sumField = this.GetExpressionDataFields(subexp, dataSetname, out subEvaluatedExpression).First();
                                string fieldGuid = Guid.NewGuid().ToString().Split('-')[0];
                                sumField.Name = "Field" + fieldGuid;
                                sumField.FunctionName = match.Groups["Function"].Value;
                                expression = expression.Replace(filedExpression, String.Empty);
                                resultExpression = resultExpression.Replace(filedExpression, "Fields!" + sumField.Name + ".Value");
                                expressionFields.Add(sumField);
                            }
                            else
                            {
                                string subEvaluatedExpression = null;
                                var fields = this.GetExpressionDataFields(subexp, dataSetname, out subEvaluatedExpression);

                                foreach (var field in fields)
                                {
                                    var fieldsCount = (from expField in expressionFields where field.FunctionName.Equals(expField.FunctionName) && field.Name.Equals(expField.Name) select expField).Count();

                                    if (fieldsCount == 0)
                                    {
                                        expressionFields.Add(field);
                                    }
                                }

                                expression = expression.Replace(filedExpression, String.Empty);
                                DataField sumField = new DataField();
                                string fieldGuid = Guid.NewGuid().ToString().Split('-')[0];
                                sumField.Name = "Field" + fieldGuid;
                                sumField.FieldName = sumField.Name;
                                sumField.FunctionName = match.Groups["Function"].Value;
                                sumField.Expression = subEvaluatedExpression;
                                resultExpression = resultExpression.Replace(filedExpression, "Fields!" + sumField.Name + ".Value");
                                expressionFields.Add(sumField);
                            }
                        }
                        else if(executeExpression)
                        {
                            string dataSourceName = null;

                            if (parameters.Length == 2)
                            {
                                dataSourceName = parameters[1].Trim().TrimStart('\"').TrimEnd('\"');
                            }
                            else if (parameters.Count() == 1 && this.pageModel.ProcessedData.DataSourceObjects.Count == 1)
                            {
                                dataSourceName = this.pageModel.ProcessedData.DataSourceObjects.First().Key;
                            }

                            if (dataSourceName != null)
                            {
                                ReportingAggEngine engine = new ReportingAggEngine();
                                engine.Model = this.pageModel;
                                engine.Fields = new List<DataField>();

                                var ViewAdv = (from view in this.pageModel.ProcessedData.DataSourceObjects
                                               where view.Key == dataSourceName
                                               select view.Value).SingleOrDefault();

                                if (ViewAdv != null)
                                {
                                    engine.DataSource = ViewAdv;
                                }

                                string subexp = parameters[0].Trim();
                                Match subMatch = new Regex(@"^Fields!\s*(?<Field>\w+)\s*.Value$").Match(subexp);

                                if (subMatch.Success)
                                {
                                    string subEvaluatedExpression = null;
                                    var sumField = this.GetExpressionDataFields(subexp, dataSourceName, out subEvaluatedExpression).First();
                                    string fieldGuid = Guid.NewGuid().ToString().Split('-')[0];
                                    sumField.Name = "Field" + fieldGuid;
                                    sumField.FunctionName = match.Groups["Function"].Value;
                                    expression = expression.Replace(filedExpression, String.Empty);
                                    resultExpression = resultExpression.Replace(filedExpression, "Fields!" + sumField.Name + ".Value");
                                    engine.Fields.Add(sumField);
                                    this.FieldValues.Add(sumField.Name, engine.GetValue());
                                }
                                else
                                {
                                    string subEvaluatedExpression = null;
                                    var fields = this.GetExpressionDataFields(subexp, dataSourceName, out subEvaluatedExpression);

                                    foreach (var field in fields)
                                    {
                                        var fieldsCount = (from expField in engine.Fields where field.FunctionName.Equals(expField.FunctionName) && field.Name.Equals(expField.Name) select expField).Count();

                                        if (fieldsCount == 0)
                                        {
                                            engine.Fields.Add(field);
                                        }
                                    }

                                    expression = expression.Replace(filedExpression, String.Empty);
                                    DataField sumField = new DataField();
                                    string fieldGuid = Guid.NewGuid().ToString().Split('-')[0];
                                    sumField.Name = "Field" + fieldGuid;
                                    sumField.FieldName = sumField.Name;
                                    sumField.FunctionName = match.Groups["Function"].Value;
                                    sumField.Expression = subEvaluatedExpression;
                                    resultExpression = resultExpression.Replace(filedExpression, "Fields!" + sumField.Name + ".Value");
                                    engine.Fields.Add(sumField);
                                    this.FieldValues.Add(sumField.Name, engine.GetValue());
                                }
                            }
                            else
                            {
                                throw new Exception("ReportItem expressions can only refer to fields within current dataset");
                            }
                        }
                        else
                        {
                            expression = expression.Replace(filedExpression, String.Empty);
                        }
                    }
                    else
                    {
                        expression = expression.Substring(0, match.Index) + expression.Substring(match.Index + match.Length);
                    }
                }
            } while (match.Success);

            if (!executeExpression)
            {
                do
                {
                    match = new Regex(@"Fields!\s*(?<Field>\w+)\s*.Value").Match(expression);

                    if (match.Success)
                    {
                        string filedExpression = match.Value;

                        string typeName = this.GetFieldType(match.Groups["Field"].Value, this.pageModel.Report, dataSetname);
                        string field = this.GetFieldName(match.Groups["Field"].Value, this.pageModel.Report, dataSetname);

                        expression = expression.Substring(0, match.Index) + expression.Substring(match.Index + match.Length);

                        DataField sumField = new DataField();
                        sumField.Name = match.Groups["Field"].Value;
                        sumField.FieldName = field;
                        sumField.FunctionName = "First";
                        sumField.DataTypeName = typeName;
                        sumField.DataSetName = dataSetname;

                        var fieldsCount = (from expField in expressionFields
                                           where (sumField.FunctionName.Equals(expField.FunctionName) && sumField.Name.Equals(expField.Name))
                                           select expField).Count();

                        if (fieldsCount == 0)
                        {
                            expressionFields.Add(sumField);
                        }
                    }
                } while (match.Success);
            }
            else if (isDataSetExec)
            {
                string dataSourceName = null;

                if (this.pageModel.ProcessedData.DataSourceObjects.Count == 1)
                {
                    dataSourceName = this.pageModel.ProcessedData.DataSourceObjects.First().Key;
                }

                ReportingAggEngine engine = new ReportingAggEngine();
                engine.Model = this.pageModel;

                do
                {
                    match = new Regex(@"Fields!\s*(?<Field>\w+)\s*.Value").Match(expression);

                    if (match.Success)
                    {
                        if (dataSourceName == null)
                        {
                            throw new Exception("ReportItem expressions can only refer to fields within current dataset");
                        }

                        string filedExpression = match.Value;

                        string typeName = this.GetFieldType(match.Groups["Field"].Value, this.pageModel.Report, dataSourceName);
                        string field = this.GetFieldName(match.Groups["Field"].Value, this.pageModel.Report, dataSourceName);

                        expression = expression.Replace(filedExpression, String.Empty);

                        DataField sumField = new DataField();
                        sumField.Name = match.Groups["Field"].Value;
                        sumField.FieldName = field;
                        sumField.FunctionName = "First";
                        sumField.DataTypeName = typeName;
                        sumField.DataSetName = dataSourceName;
                        engine.Fields = new List<DataField>();

                        var ViewAdv = (from view in this.pageModel.ProcessedData.DataSourceObjects
                                       where view.Key == dataSourceName
                                       select view.Value).SingleOrDefault();

                        if (ViewAdv != null)
                        {
                            engine.DataSource = ViewAdv;
                        }

                        string fieldGuid = Guid.NewGuid().ToString().Split('-')[0];
                        sumField.Name = "Field" + fieldGuid;
                        resultExpression = resultExpression.Replace(filedExpression, "Fields!" + sumField.Name + ".Value");
                        engine.Fields.Add(sumField);
                        this.FieldValues.Add(sumField.Name, engine.GetValue());
                    }
                } while (match.Success);
            }

            return expressionFields;
        }

        public string GetCalculationValue(DataField field, string dataSetName)
        {
            ReportingAggEngine engine = new ReportingAggEngine();
            engine.Model = this.pageModel;
            engine.Fields = new List<DataField>();

            var ViewAdv = (from view in this.pageModel.ProcessedData.DataSourceObjects
                           where view.Key == dataSetName
                           select view.Value).SingleOrDefault();

            if (ViewAdv != null)
            {
                engine.DataSource = ViewAdv;
                return engine.GetValue().ToString();
            }

            return string.Empty;
        }

        public string GetFieldsSets(string expression)
        {
            Match match = null;
            do
            {
                match = new Regex(@"(?<Data>(?<Function>\w+)\s*\(Fields!\s*(?<Field>\w+).Value,\s*\" + "\"" + @"(?<DataSet>\w+)\" + "\"" + @"\))").Match(expression);
                if (match.Success)
                {
                    string filedExpression = match.Value;
                    DataField sumField = new DataField();
                    sumField.FunctionName = match.Groups["Function"].Value;
                    sumField.FieldName = match.Groups["Field"].Value;
                    string dataSet = match.Groups["DataSet"].Value;
                    expression = expression.Replace(filedExpression, this.GetCalculationValue(sumField, dataSet));
                }
            } while (match.Success);

            return expression;
        }

        public bool IsAggregateFunction(string functionName)
        {
            switch (functionName.ToLower())
            {
                case "sum":
                case "avg":
                case "count":
                case "countdistinct":
                case "stdev":
                case "stdevp":
                case "max":
                case "min":
                case "first":
                case "last":
                case "var":
                case "varp":
                    return true;
            }

            return false;
        }

        #region  Method

        public object GetParameterPlaceHolderValue(string parameter, string paramData)
        {
            bool isValue = (paramData.Equals("Value")) ? true : false;

            var parameters = from modelParamter in this.pageModel.ParameterDetails where modelParamter.Name.Equals(parameter) select modelParamter;

            if (parameters.Count() > 0)
            {
                var param = parameters.First();

                if (param.Value.Count > 0)
                {
                    if (isValue)
                    {
                        return (param.IsMultiValue) ? param.Value : param.Value.First();
                    }
                    else
                    {
                        return (param.IsMultiValue) ? param.Label : param.Label.First();
                    }
                }

                throw new Exception("The " + param.Name + " parameter missing as Values");
            }
            return null;
        }

        #endregion

        internal string GetFormattedText(object value, string format,string language)
        {
            if (value != null)
            {
                CultureInfo info = new CultureInfo("en-US");

                if (!string.IsNullOrEmpty(language))
                {
                    info = new CultureInfo(language);
                }

                string text = value.ToString();

                if (!string.IsNullOrEmpty(format) || !string.IsNullOrEmpty(language))
                {
                    double numberData = 0;
                    DateTime dateTime = DateTime.Now;

                    if (double.TryParse(text, out numberData))
                    {
                        return numberData.ToString(format, info);
                    }

                    if (DateTime.TryParse(text, out dateTime))
                    {
                        try
                        {
                            return dateTime.ToString(format, info);
                        }
                        catch
                        {
                            return text;
                        }
                    }
                }

                return text;
            }

            return String.Empty;
        }

        internal string ParsedFieldName(string fieldNameContainer)
        {
            string parsedFieldName = fieldNameContainer;
            if (fieldNameContainer.Contains('!') && fieldNameContainer.Contains('.'))
            {
                int indexOfExclamatory = fieldNameContainer.IndexOf('!');
                int indexOfDot = fieldNameContainer.IndexOf('.');
                int stringLength = (indexOfDot - indexOfExclamatory) - 1;
                int startingIndex = indexOfExclamatory + 1;

                parsedFieldName = fieldNameContainer.Substring(startingIndex, stringLength);
            }

            return parsedFieldName;
        }

        internal string GetFieldName(string column, ReportDefinition report, string DataSetName)
        {
            string fieldValue = (from dataSetThis in report.DataSets
                                 from DataSetfield in dataSetThis.Fields
                                 where DataSetfield.Name == column
                                 where dataSetThis.Name == DataSetName
                                 select DataSetfield.DataField).FirstOrDefault();
            if (fieldValue == null)
            {
                return column;
            }
            return fieldValue;
        }

        internal string GetFieldName(string column, string DataSetName)
        {
            string fieldValue = (from dataSetThis in this.pageModel.Report.DataSets
                                 from DataSetfield in dataSetThis.Fields
                                 where DataSetfield.Name == column
                                 where dataSetThis.Name == DataSetName
                                 select DataSetfield.DataField).FirstOrDefault();
            if (fieldValue == null)
            {
                return column;
            }
            return fieldValue;
        }

        internal string GetFieldType(string column, ReportDefinition report, string DataSetName)
        {
            string typeName = (from dataSetThis in report.DataSets
                               from DataSetfield in dataSetThis.Fields
                               where DataSetfield.Name == column
                               where dataSetThis.Name == DataSetName
                               select DataSetfield.TypeName).FirstOrDefault();
            if (typeName == null)
            {
                return typeof(string).ToString();
            }
            return typeName;
        }

        internal class BinaryOperator
        {
            public string Operator { get; set; }

            public int Precedence { get; set; }

            public BinaryOperator(string strOperator)
            {
                this.Operator = strOperator;
                this.Precedence = this.OperatorPrecedence(this.Operator);
            }

            internal int OperatorPrecedence(string strOperator)
            {
                switch (strOperator)
                {
                    case "*":
                    case "/":
                    case "%":
                        return 0;
                    case "+":
                    case "-":
                        return 1;
                    case ">>":
                    case "<<":
                        return 2;
                    case "<":
                    case "<=":
                    case ">":
                    case ">=":
                    case "==":
                    case "=":
                    case "!=":
                        return 4;
                    case "&":
                        return 5;
                    case "^":
                        return 6;
                    case "|":
                        return 7;
                    case "&&":
                        return 8;
                    case "||":
                        return 9;
                }

                if (ExpressionMatch.LikeOperator.Equals(strOperator))
                {
                    return 3;
                }
                if (ExpressionMatch.IsOperator.Equals(strOperator))
                {
                    return 4;
                }
                if (ExpressionMatch.XorOperator.Equals(strOperator))
                {
                    return 8;
                }

                throw new Exception("Invalid Expression : operator " + strOperator);
            }
        }

        internal class BinaryOperatorQueue
        {
            private List<object> operatorlist = new List<object>();

            public BinaryOperatorQueue(List<object> expressionlist)
            {
                foreach (object item in expressionlist)
                {
                    Enqueue(item as BinaryOperator);
                }
            }

            public void Enqueue(BinaryOperator binaryOperator)
            {
                if (binaryOperator == null)
                    return;

                bool queued = false;

                for (int x = 0; x < operatorlist.Count && !queued; x++)
                {
                    if (((BinaryOperator)operatorlist[x]).Precedence > binaryOperator.Precedence)
                    {
                        operatorlist.Insert(x, binaryOperator);
                        queued = true;
                    }
                }

                if (!queued)
                {
                    operatorlist.Add(binaryOperator);
                }
            }

            public BinaryOperator Dequeue()
            {
                if (operatorlist.Count == 0)
                {
                    return null;
                }

                BinaryOperator binaryOperator = (BinaryOperator)operatorlist[0];
                operatorlist.RemoveAt(0);
                return binaryOperator;
            }

        }

        internal class UnaryOperator
        {
            public string Operator { get; set; }

            public UnaryOperator(string strOperator)
            {
                this.Operator = strOperator;
            }
        }

        internal class ExpressionEval
        {
            #region Private Members

            List<object> expressionlist = new List<object>();
            string expression = "";
            bool isParsed;
            ExpressionEngine expressionEngine = null;


            #endregion

            #region Construction

            public ExpressionEval()
            {
            }

            public ExpressionEval(string expression)
            {
                this.Expression = expression;
            }

            public ExpressionEval(string expression, ExpressionEngine engine)
            {
                this.Expression = expression;
                this.expressionEngine = engine;
            }

            #endregion

            #region Properties

            public string Expression
            {
                get
                {
                    return expression;
                }
                set
                {
                    expression = value.Trim();
                    isParsed = false;
                    expressionlist.Clear();
                }
            }

            #endregion

            #region Methods

            public object Evaluate()
            {
                if ("" + Expression == "")
                {
                    return 0;
                }

                return ExecuteEvaluation();
            }

            public static object Evaluate(string expressionString)
            {
                ExpressionEval expression = new ExpressionEval(expressionString);
                return expression.Evaluate();
            }

            private object ExecuteEvaluation()
            {
                if (!isParsed)
                {
                    for (int x = 0; x < Expression.Length; x = NextToken(x)) ;
                }

                isParsed = true;

                return EvaluateList();
            }

            private int NextToken(int startIndex)
            {
                Match RegexObject = null;
                int totalIndex = startIndex;
                object val = null;

                //Check for preceeding white space from last token index
                Match expMatchedObject = ExpressionMatch.WhiteSpace.Match(this.Expression, startIndex);

                if (expMatchedObject.Success && expMatchedObject.Index == startIndex)
                {
                    return startIndex + expMatchedObject.Length;
                }

                //Check logical Operator               
                expMatchedObject = ExpressionMatch.LogicalOperator.Match(Expression, startIndex);

                if (expMatchedObject.Success && expMatchedObject.Index == startIndex)
                {
                    string op = string.Empty;

                    switch (expMatchedObject.Value.Trim().ToLower())
                    {
                        case "\\":
                            op = "/";
                            break;
                        case "and":
                        case "andalso":
                            op = ExpressionMatch.AndOperator;
                            break;
                        case "orelse":
                        case "or":
                            op = ExpressionMatch.OrOperator;
                            break;
                        case "mod":
                            op = ExpressionMatch.ModOperator;
                            break;
                        case "like":
                            op = ExpressionMatch.LikeOperator;
                            break;
                        case "xor":
                            op = ExpressionMatch.XorOperator;
                            break;
                        case "is":
                            op = ExpressionMatch.IsOperator;
                            break;
                    }

                    if (!String.IsNullOrEmpty(op))
                    {
                        this.expression = this.expression.Substring(0, startIndex) + op + this.expression.Substring(startIndex + expMatchedObject.Length);
                        return startIndex;
                    }
                }

                //Check Parenthesis
                expMatchedObject = ExpressionMatch.Parenthesis.Match(Expression, startIndex);

                if (expMatchedObject.Success)
                {
                    RegexObject = expMatchedObject;
                }               

                //Check Modulefunction
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.CodeMatchFunction.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                    }
                }

                //Check Function
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.Function.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                    }
                }

                //Check ObjectFunction
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.ObjectFunction.Match(Expression, startIndex);

                    if (expMatchedObject.Success && expMatchedObject.Value.StartsWith(".") && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = new ObjectFunction(this.expressionEngine);
                    }
                }

                //Check Unary Operator
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.UnaryOp.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = new UnaryOperator(expMatchedObject.Value);
                    }
                }


                //Check Boolean
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.Boolean.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = bool.Parse(expMatchedObject.Value);
                    }
                }

                //Check DateTime
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.DateTime.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = Convert.ToDateTime(expMatchedObject.Groups["DateString"].Value, CultureInfo.CurrentCulture);
                    }
                }
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.DateTime2.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = Convert.ToDateTime(expMatchedObject.Groups["Date"].Value, CultureInfo.CurrentCulture);
                    }
                }

                //Check Timespan
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.TimeSpan.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = new TimeSpan(
                            int.Parse("0" + expMatchedObject.Groups["Days"].Value),
                            int.Parse(expMatchedObject.Groups["Hours"].Value),
                            int.Parse(expMatchedObject.Groups["Minutes"].Value),
                            int.Parse("0" + expMatchedObject.Groups["Seconds"].Value),
                            int.Parse("0" + expMatchedObject.Groups["Milliseconds"].Value)
                        );
                    }
                }

                //Check Numeric
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.Numeric.Match(Expression, startIndex);
                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        while (expMatchedObject.Success && ("" + expMatchedObject.Value == ""))
                        {
                            expMatchedObject = expMatchedObject.NextMatch();
                        }

                        if (expMatchedObject.Success)
                        {
                            RegexObject = expMatchedObject;
                            val = double.Parse(expMatchedObject.Value, CultureInfo.CurrentCulture);
                        }
                    }
                }

                //Check Parameter
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.Parameter.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = this.expressionEngine.GetParameterPlaceHolderValue(expMatchedObject.Groups["Parameter"].Value, expMatchedObject.Groups["Data"].Value);
                    }
                }

                //Check field
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.Field.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        Dictionary<string, object> fieldValues = this.expressionEngine.FieldValues;
                        val = fieldValues.Keys.Contains(RegexObject.Groups["Field"].Value) ? fieldValues[RegexObject.Groups["Field"].Value] : null;
                    }
                }

                //Check Enum
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.Enum.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = this.GetEnumValue(RegexObject);
                    }
                }

                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    //Check String
                    expMatchedObject = ExpressionMatch.String.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = expMatchedObject.Groups["String"].Value.Replace("\\\"", "\"");
                    }
                }

                //Check Binary Operator
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.BinaryOp.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = new BinaryOperator(expMatchedObject.Value);
                    }
                }

                //Check Const
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.Const.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = this.GetConstValue(RegexObject);
                    }
                }

                //Check Execution match
                if (RegexObject == null || RegexObject.Index > startIndex)
                {
                    expMatchedObject = ExpressionMatch.ExecutionTimeMatch.Match(Expression, startIndex);

                    if (expMatchedObject.Success && (RegexObject == null || expMatchedObject.Index < RegexObject.Index))
                    {
                        RegexObject = expMatchedObject;
                        val = this.expressionEngine.pageModel.ExecutionTime;
                    }
                }

                if (RegexObject == null)
                {
                    throw new Exception("Invalid Expression :" + Expression);
                }

                if (RegexObject.Index != startIndex)
                {
                    throw new Exception("Invalid Expression :" + Expression);
                }

                if (RegexObject.Value == "(" || RegexObject.Value.EndsWith("("))
                {
                    totalIndex = RegexObject.Index + RegexObject.Length;
                    int level = 1;
                    bool isInQuotes = false;

                    while (level > 0)
                    {
                        if (totalIndex >= Expression.Length)
                        {
                            throw new Exception("Invalid Expression :" + Expression);
                        }
                        if (!isInQuotes && Expression[totalIndex] == ')')
                        {
                            level--;
                        }
                        if (!isInQuotes && Expression[totalIndex] == '(')
                        {
                            level++;
                        }

                        if (Expression[totalIndex] == '"' && (totalIndex == 0 || Expression[totalIndex - 1] != '\\'))
                        {
                            isInQuotes = !isInQuotes;
                        }

                        totalIndex++;
                    }
                    if (RegexObject.Value == "(")
                    {
                        ExpressionEval expr = new ExpressionEval(
                            Expression.Substring(RegexObject.Index + 1, totalIndex - RegexObject.Index - 2), this.expressionEngine
                        );

                        expressionlist.Add(expr);
                    }
                    else
                    {
                        if (val != null && val is ObjectFunction)
                        {
                            ObjectFunction objFunction = val as ObjectFunction;
                            objFunction.Expression = Expression.Substring(RegexObject.Index, (totalIndex) - RegexObject.Index);
                            expressionlist.Add(objFunction);
                        }
                        else
                        {
                            FunctionEval func = new FunctionEval(
                                Expression.Substring(RegexObject.Index, (totalIndex) - RegexObject.Index), this.expressionEngine
                            );

                            expressionlist.Add(func);
                        }
                    }
                }
                else
                {
                    totalIndex = RegexObject.Index + RegexObject.Length;
                    expressionlist.Add(val);
                }

                return totalIndex;
            }

            private object EvaluateList()
            {
                List<object> list = (List<object>)expressionlist;

                //Do the unary operators first
                for (int x = 0; x < list.Count; x++)
                {
                    if (list[x] is ObjectFunction)
                    {
                        ObjectFunction objFucntion = (ObjectFunction)list[x];
                        list[x - 1] = objFucntion.Evaluate(list[x - 1]);
                        list.RemoveAt(x);
                        x--;
                    }
                }

                //Do the unary operators first
                for (int x = 0; x < list.Count; x++)
                {
                    if (list[x] is UnaryOperator)
                    {
                        list[x] = PerformUnaryOp(
                            (UnaryOperator)list[x],
                            list[x + 1]
                        );
                        list.RemoveAt(x + 1);
                    }
                }

                //Get the queued binary operations
                BinaryOperatorQueue oprQueue = new BinaryOperatorQueue(list);

                for (int x = 1; x < list.Count; x += 2)
                {
                    if (list[x] is BinaryOperator)
                    {
                        if (x + 1 == list.Count)
                        {
                            throw new Exception("Invalid Expression :" + Expression);
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid Expression :" + Expression);
                    }
                }

                BinaryOperator binOperator = oprQueue.Dequeue();

                while (binOperator != null)
                {
                    int index = list.IndexOf(binOperator);
                    list[index - 1] = PerformBinaryOp((BinaryOperator)list[index], list[index - 1], list[index + 1]);
                    list.RemoveAt(index);
                    list.RemoveAt(index);
                    binOperator = oprQueue.Dequeue();
                }

                object retExpression = null;

                if (list[0] is FunctionEval)
                {
                    retExpression = ((FunctionEval)list[0]).Evaluate();
                }
                else if (list[0] is ExpressionEval)
                {
                    retExpression = ((ExpressionEval)list[0]).Evaluate();
                }
                else
                {
                    retExpression = list[0];
                }

                return retExpression;
            }

            private static object PerformBinaryOp(BinaryOperator binOperator, object var1, object var2)
            {
                if (var1 is FunctionEval)
                {
                    var1 = ((FunctionEval)var1).Evaluate();
                }
                else if (var1 is ExpressionEval)
                {
                    var1 = ((ExpressionEval)var1).Evaluate();
                }

                if (var2 is FunctionEval)
                {
                    var2 = ((FunctionEval)var2).Evaluate();
                }
                else if (var2 is ExpressionEval)
                {
                    var2 = ((ExpressionEval)var2).Evaluate();
                }


                switch (binOperator.Operator)
                {
                    case "*":
                        return (Convert.ToDouble(var1, CultureInfo.CurrentCulture) *
                                      Convert.ToDouble(var2, CultureInfo.CurrentCulture));
                    case "/":
                        return (Convert.ToDouble(var1, CultureInfo.CurrentCulture) /
                                      Convert.ToDouble(var2, CultureInfo.CurrentCulture));
                    case "%":
                        return (Convert.ToInt64(var1, CultureInfo.CurrentCulture) %
                                      Convert.ToInt64(var2, CultureInfo.CurrentCulture));
                    case "<<":
                        return (Convert.ToInt64(var1, CultureInfo.CurrentCulture) <<
                                       Convert.ToInt32(var2, CultureInfo.CurrentCulture));
                    case ">>":
                        return (Convert.ToInt64(var1, CultureInfo.CurrentCulture) >>
                                       Convert.ToInt32(var2, CultureInfo.CurrentCulture));
                    case "+":
                    case "-":
                    case "<":
                    case "<=":
                    case ">":
                    case ">=":
                    case "==":
                    case "=":
                    case "!=":
                    case "&":
                        return DoSpecialOperator(binOperator, var1, var2);
                    case "^":
                        return (Convert.ToUInt64(var1, CultureInfo.CurrentCulture) ^
                                      Convert.ToUInt64(var2, CultureInfo.CurrentCulture));
                    case "|":
                        return (Convert.ToUInt64(var1, CultureInfo.CurrentCulture) |
                                      Convert.ToUInt64(var2, CultureInfo.CurrentCulture));
                    case "&&":
                        return (Convert.ToBoolean(var1, CultureInfo.CurrentCulture) &&
                                       Convert.ToBoolean(var2, CultureInfo.CurrentCulture));
                    case "||":
                        return (Convert.ToBoolean(var1, CultureInfo.CurrentCulture) ||
                                       Convert.ToBoolean(var2, CultureInfo.CurrentCulture));
                }

                if (binOperator.Operator.Equals(ExpressionMatch.LikeOperator))
                {
                    return DoSpecialOperator(binOperator, var1, var2);
                }
                else if (binOperator.Operator.Equals(ExpressionMatch.IsOperator))
                {
                    return Is(var1, var2);
                }
                else if (binOperator.Operator.Equals(ExpressionMatch.XorOperator))
                {
                    return XOr(Convert.ToBoolean(var1, CultureInfo.CurrentCulture), Convert.ToBoolean(var2, CultureInfo.CurrentCulture));
                }

                throw new Exception("Invalid Expression :" + binOperator.Operator);
            }                                              

            private static bool Is(object var1, object var2)
            {
                try
                {
                    return object.ReferenceEquals(var1, var2);
                }
                catch
                {
                    return false;
                }
            }

            private static bool XOr(bool var1, bool var2)
            {
                if (var1 == var2)
                {
                    return true;
                }

                return false;
            }


            private static object DoSpecialOperator(BinaryOperator binaryOperator, object var1, object var2)
            {
                DateTime date1;
                DateTime date2;

                if (var1 != null && var2 != null)
                {
                    if (DateTime.TryParse(var1.ToString(), out date1) && DateTime.TryParse(var2.ToString(), out date2))
                    {
                        switch (binaryOperator.Operator)
                        {
                            case "+":
                                throw new Exception("Invalid Expression : " + binaryOperator.Operator);
                            case "-":
                                return date1 - date2;
                            case "<":
                                return date1 < date2;
                            case "<=":
                                return date1 <= date2;
                            case ">":
                                return date1 > date2;
                            case ">=":
                                return date1 >= date2;
                            case "==":
                            case "=":
                                return date1 == date2;
                            case "!=":
                                return date1 != date2;
                        }
                    }
                }

                double float1, float2;
                if (var1 != null && var2 != null)
                {
                    if (double.TryParse(var1.ToString(), out float1) && double.TryParse(var2.ToString(), out float2))
                    {
                        switch (binaryOperator.Operator)
                        {
                            case "+":
                                return float1 + float2;
                            case "-":
                                return float1 - float2;
                            case "<":
                                return float1 < float2;
                            case "<=":
                                return float1 <= float2;
                            case ">":
                                return float1 > float2;
                            case ">=":
                                return float1 >= float2;
                            case "==":
                            case "=":
                                return float1 == float2;
                            case "!=":
                                return float1 != float2;
                        }
                    }
                }

                try
                {
                    string string1 = "" + var1;
                    string string2 = "" + var2;
                    string string3 = "" + binaryOperator;

                    switch (binaryOperator.Operator)
                    {
                        case "+":
                            return string1 + string2;
                        case "-":
                            return string1 + string3 + string2;
                        case "<":
                            return string1.CompareTo(string2) < 0;
                        case "<=":
                            return string1.CompareTo(string2) < 0 || string1 == string2;
                        case ">":
                            return string1.CompareTo(string2) > 0;
                        case ">=":
                            return string1.CompareTo(string2) > 0 || string1 == string2; ;
                        case "==":
                        case "=":
                            return string1 == string2;
                        case "!=":
                            return string1 != string2;
                        case "&":
                            return string1 + string2;
                    }

                    if (binaryOperator.Operator.Equals(ExpressionMatch.LikeOperator))
                    {
                        string2 = "^" + string2;
                        string2 = string2 + "$";

                        if (string2.Contains("*"))
                        {
                            string2 = string2.Replace("*", @"(\s*)(\w*)(\s*)");
                        }
                        if (string2.Contains("#"))
                        {
                            string2 = string2.Replace("#", @"\d");
                        }
                        if (string2.Contains("!"))
                        {
                            string2 = string2.Replace("!", @"^");
                        }
                        if (string2.Contains("?"))
                        {
                            string2 = string2.Replace("?", @"(\s|\S)");
                        }

                        Regex likeReg = new Regex(string2);
                        return likeReg.Match(string1).Success;
                    }

                    throw new Exception("Invalid Expression : " + binaryOperator.Operator);
                }
                catch
                {
                    throw new Exception("Invalid Expression : " + binaryOperator.Operator);
                }
            }

            private static object PerformUnaryOp(UnaryOperator unaryOperator, object var)
            {
                if (var is ExpressionEval)
                {
                    var = ((ExpressionEval)var).Evaluate();
                }
                else if (var is FunctionEval)
                {
                    var = ((FunctionEval)var).Evaluate();
                }

                switch (unaryOperator.Operator)
                {
                    case "+":
                        return (Convert.ToDouble(var, CultureInfo.CurrentCulture));
                    case "-":
                        return (-Convert.ToDouble(var, CultureInfo.CurrentCulture));
                    case "!":
                        return (!Convert.ToBoolean(var, CultureInfo.CurrentCulture));
                    case "~":
                        return (~Convert.ToUInt64(var, CultureInfo.CurrentCulture));
                }
                throw new Exception("Invalid Expression : " + unaryOperator.Operator);
            }

            #endregion

            #region Constant methods
            private object GetConstValue(Match regMatch)
            {
                string enumName = regMatch.Groups["Name"].Value;

                switch (enumName.ToLower())
                {
                    case "now":
                        return DateTime.Now;
                    case "nothing":
                        return null;
                }

                return null;
            }
            #endregion

            #region EnumMethods

            private int GetEnumValue(Match regMatch)
            {
                string enumName = regMatch.Groups["Enum"].Value;
                string enumValue = regMatch.Groups["Value"].Value;

                switch (enumName.ToLower())
                {
                    case "comparemethod":
                        return this.GetComparemeMethodEnum(enumValue);
                    case "duedate":
                        return this.GetDueDateEnum(enumValue);
                    case "vbstrconv":
                        return this.GetVbStrConvEnum(enumValue);
                    case "firstdayofweek":
                        return this.GetFirstDayOfWeekEnum(enumValue);
                    case "dateformat":
                        return this.GetDateFormatEnum(enumValue);
                    case "dateinterval":
                        return this.GetDateIntervalEnum(enumValue);
                }

                return 0;
            }

            private int GetComparemeMethodEnum(string value)
            {
                if (value.ToLower().Equals("binary"))
                {
                    return 0;
                }

                return 1;
            }

            private int GetDueDateEnum(string value)
            {
                switch (value.ToLower())
                {
                    case "endofperiod":
                        return 0;
                    case "beginofperiod":
                        return 1;
                }

                return 0;
            }

            private int GetVbStrConvEnum(string value)
            {
                switch (value.ToLower())
                {
                    case "uppercase":
                        return 1;
                    case "lowercase":
                        return 2;
                    case "propercase":
                        return 3;
                    case "wide":
                        return 4;
                    case "narrow":
                        return 8;
                    case "katakana":
                        return 16;
                    case "hiragana":
                        return 32;
                    case "simplifiedchinese":
                        return 256;
                    case "traditionalchinese":
                        return 512;
                    case "linguisticcasing":
                        return 1024;
                }

                return 0;
            }

            private int GetFirstDayOfWeekEnum(string value)
            {
                switch (value.ToLower())
                {
                    case "sunday":
                        return 1;
                    case "monday":
                        return 2;
                    case "tuesday":
                        return 3;
                    case "wednesday":
                        return 4;
                    case "thursday":
                        return 5;
                    case "friday":
                        return 6;
                    case "saturday":
                        return 7;
                }

                return 0;
            }

            private int GetDateFormatEnum(string value)
            {
                switch (value.ToLower())
                {
                    case "longdate":
                        return 1;
                    case "shortdate":
                        return 2;
                    case "longtime":
                        return 3;
                    case "shorttime":
                        return 4;
                }
                return 0;
            }

            private int GetDateIntervalEnum(string value)
            {
                switch (value.ToLower().Trim())
                {
                    case "day":
                        return 4;
                    case "hour":
                        return 7;
                    case "minute":
                        return 8;
                    case "month":
                        return 2;
                    case "quarter":
                        return 1;
                    case "second":
                        return 9;
                    case "weekday":
                        return 6;
                    case "weekofyear":
                        return 5;
                }

                return 0;
            }

            #endregion
        }


        internal class ObjectFunction
        {
            public string Expression { get; set; }

            public ExpressionEngine ExpressionEngine { get; set; }

            public ObjectFunction(ExpressionEngine expressionEngine)
            {
                this.ExpressionEngine = expressionEngine;
            }

            internal object Evaluate(object var)
            {
                Match m = ExpressionMatch.Function.Match(this.Expression);
                object[] pars = this.GetParameters(m);
                string function = m.Groups["Function"].Value;

                if (var is ExpressionEval)
                {
                    var = ((ExpressionEval)var).Evaluate();
                }
                else if (var is FunctionEval)
                {
                    var = ((FunctionEval)var).Evaluate();
                }

                switch (function.ToLower(CultureInfo.CurrentCulture))
                {
                    case "tostring":
                        return ToStringEval(var, pars);
                }

                throw new Exception("Invalid Expression :");
            }

            private string ToStringEval(object var, object[] parameters)
            {
                if (parameters != null && parameters.Length > 0)
                {
                    object p = parameters.First();

                    if (p is ExpressionEval)
                    {
                        p = ((ExpressionEval)p).Evaluate();
                    }

                    string format = p.ToString();

                    if (var is DateTime)
                    {
                        DateTime date = (DateTime)var;
                        return date.ToString(format);
                    }
                    else if (var is double || var is float || var is Int32 || var is Int16 || var is Int64 ||
                        var is UInt32 || var is UInt16 || var is UInt64)
                    {
                        double value = 0;

                        if (!string.IsNullOrEmpty(var.ToString()))
                        {
                            value = double.Parse(var.ToString());
                        }
                        return value.ToString(format);
                    }
                }

                return (var != null) ? var.ToString() : string.Empty;
            }

            private object[] GetParameters(Match m)
            {
                string strParameters = "";
                int index = m.Index + m.Length;
                int level = 1;
                int lastIndex = 0;
                bool isInQuotes = false;
                List<object> retExprs = new List<object>();

                //Get the parameter string
                while (level > 0)
                {
                    if (index >= this.Expression.Length)
                        throw new ArgumentException("Missing ')' in Expression");

                    if (!isInQuotes && this.Expression[index] == ')')
                    {
                        level--;
                    }
                    if (!isInQuotes && this.Expression[index] == '(')
                    {
                        level++;
                    }

                    if (this.Expression[index] == '"' && (index == 0 || this.Expression[index - 1] != '\\'))
                    {
                        isInQuotes = !isInQuotes;
                    }

                    if (level > 0)
                    {
                        index++;
                    }
                }

                strParameters = this.Expression.Substring(m.Index + m.Length, index - (m.Index + m.Length));

                if ("" + strParameters == "")
                    return null;

                isInQuotes = false;
                for (index = 0; index < strParameters.Length; index++)
                {
                    if (!isInQuotes && strParameters[index] == ')')
                    {
                        level--;
                    }
                    if (!isInQuotes && strParameters[index] == '(')
                    {
                        level++;
                    }

                    if (strParameters[index] == '"' && (index == 0 || strParameters[index - 1] != '\\'))
                    {
                        isInQuotes = !isInQuotes;
                    }

                    if (!isInQuotes && level == 0 && strParameters[index] == ',')
                    {
                        retExprs.Add(strParameters.Substring(lastIndex, index - lastIndex));
                        lastIndex = index + 1;
                    }
                }

                retExprs.Add(strParameters.Substring(lastIndex, index - lastIndex));

                for (index = 0; index < retExprs.Count; index++)
                {
                    ExpressionEval eval = new ExpressionEval(retExprs[index].ToString(), this.ExpressionEngine);
                    retExprs[index] = eval;
                }

                return retExprs.ToArray();
            }

        }

        internal class FunctionEval
        {
            #region Private Members

            private string expression = "";
            private string function = "";
            private bool isParsed;
            private object[] pars;
            private ExpressionEngine expressionEngine = null;
            private bool isCodeModuleFunction = false;

            #endregion

            #region Properties

            public string Expression
            {
                get { return expression; }
                set
                {
                    expression = value;
                    function = "";
                    isParsed = false;
                    pars = null;
                }
            }

            #endregion

            #region Constructor

            public FunctionEval()
            {
            }

            public FunctionEval(string expression, ExpressionEngine engine)
            {
                Expression = expression;
                this.expressionEngine = engine;
            }

            #endregion

            #region Methods

            public object Evaluate()
            {
                isCodeModuleFunction = false;
                object retExp = null;
                if (!isParsed)
                {
                    StringBuilder strbExpression = new StringBuilder(Expression);
                    string strNext = strbExpression.ToString();
                    Match m = ExpressionMatch.CodeMatchFunction.Match(strNext);

                    if (m.Success)
                    {
                        isCodeModuleFunction = true;
                        pars = GetParameters(m);
                        function = m.Value.Trim().TrimEnd('(');
                    }
                    else
                    {
                        m = ExpressionMatch.Function.Match(strNext);
                        if (m.Success)
                        {
                            pars = GetParameters(m);
                            function = m.Groups["Function"].Value;
                        }
                    }
                    isParsed = true;
                }
                retExp = ExecuteFunction(function, pars);
                return retExp;
            }

            private object[] GetParameters(Match m)
            {
                string strParameters = "";
                int index = m.Index + m.Length;
                int level = 1;
                int lastIndex = 0;
                bool isInQuotes = false;
                List<object> retExprs = new List<object>();

                //Get the parameter string
                while (level > 0)
                {
                    if (index >= Expression.Length)
                        throw new ArgumentException("Missing ')' in Expression");

                    if (!isInQuotes && Expression[index] == ')')
                    {
                        level--;
                    }
                    if (!isInQuotes && Expression[index] == '(')
                    {
                        level++;
                    }

                    if (Expression[index] == '"' && (index == 0 || Expression[index - 1] != '\\'))
                    {
                        isInQuotes = !isInQuotes;
                    }

                    if (level > 0)
                    {
                        index++;
                    }
                }

                strParameters = Expression.Substring(m.Index + m.Length, index - (m.Index + m.Length));

                if ("" + strParameters == "")
                    return null;

                isInQuotes = false;
                for (index = 0; index < strParameters.Length; index++)
                {
                    if (!isInQuotes && strParameters[index] == ')')
                    {
                        level--;
                    }
                    if (!isInQuotes && strParameters[index] == '(')
                    {
                        level++;
                    }

                    if (strParameters[index] == '"' && (index == 0 || strParameters[index - 1] != '\\'))
                    {
                        isInQuotes = !isInQuotes;
                    }

                    if (!isInQuotes && level == 0 && strParameters[index] == ',')
                    {
                        retExprs.Add(strParameters.Substring(lastIndex, index - lastIndex));
                        lastIndex = index + 1;
                    }
                }

                retExprs.Add(strParameters.Substring(lastIndex, index - lastIndex));

                string function = m.Groups["Function"].Value.ToLower();


                if (!(this.expressionEngine.IsAggregateFunction(function)) || isCodeModuleFunction)
                {
                    for (index = 0; index < retExprs.Count; index++)
                    {
                        if (!string.IsNullOrEmpty(retExprs[index].ToString()))
                        {
                            ExpressionEval eval = new ExpressionEval(retExprs[index].ToString(), this.expressionEngine);
                            retExprs[index] = eval;
                        }
                    }
                }
                else
                {
                    return retExprs.ToArray();
                }

                return retExprs.Where(p => p is ExpressionEval).ToArray();
            }

            private object ExecuteFunction(string name, object[] p)
            {
                object[] parameters = null;
                if (p != null)
                {
                    parameters = (object[])p.Clone();
                    for (int x = 0; x < parameters.Length; x++)
                    {
                        if (parameters[x] is ExpressionEval)
                            parameters[x] = ((ExpressionEval)parameters[x]).Evaluate();
                    }
                }

                if (isCodeModuleFunction)
                {
                    if (this.expressionEngine != null)
                    {
                        return this.expressionEngine.EvalValue(name, parameters);
                    }
                    else
                    {
                        return "#Error";
                    }
                }

                switch (name.ToLower(CultureInfo.CurrentCulture))
                {
                    // Logical function
                    case "not":
                        return Not(parameters);
                    // Report Math functions
                    case "abs":
                        return Math.Abs(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "acos":
                        return Math.Acos(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "asin":
                        return Math.Asin(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "atan":
                        return Math.Atan(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "atan2":
                        return Math.Atan2(Convert.ToDouble(parameters[0]), Convert.ToDouble(parameters[1]));
#if !SILVERLIGHT
                    case "bigmul":
                        return Math.BigMul(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]));
#endif
                    case "ceiling":
                        return Math.Ceiling(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "cos":
                        return Math.Cos(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "cosh":
                        return Math.Cosh(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "exp":
                        return Math.Exp(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "fix":
                        return Math.Ceiling(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "floor":
                        return Math.Floor(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "int":
                        return Math.Floor(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "log":
                        return (parameters.Length > 1) ?
                                        Math.Log(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture), Convert.ToDouble(parameters[1], CultureInfo.CurrentCulture)) :
                                        Math.Log(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "log10":
                        return Math.Log10(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "pow":
                        return Math.Pow(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture), Convert.ToDouble(parameters[1], CultureInfo.CurrentCulture));
                    case "round":
                        return (parameters.Length > 1) ?
                                          Math.Round(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture), Convert.ToInt32(parameters[1], CultureInfo.CurrentCulture)) :
                                          Math.Round(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "rnd":
                        return new Random().NextDouble();
                    case "sign":
                        return Convert.ToInt32(parameters[0]) > 0 ? 1 : -1;
                    case "sin":
                        return Math.Sin(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "sinh":
                        return Math.Sinh(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "sqrt":
                        return Math.Sqrt(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "tan":
                        return Math.Tan(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));
                    case "tanh":
                        return Math.Tanh(Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture));

                    //Text
                    case "asc":
                    case "ascw":
                        return Asc(parameters);
                    case "chr":
                    case "chrw":
                        return Chr(parameters);
                    case "filter":
                        return Filter(parameters);
                    case "formatdatetime":
                        return FormatDateTime(parameters);
                    case "formatpercent":
                        return FormatPercentage(parameters);
                    case "formatnumber":
                        return FormatNumber(parameters);
                    case "format":
                        return Format(parameters);
                    case "formatcurrency":
                        return FormatCurrency(parameters);
                    case "getchar":
                        return GetChar(parameters);
                    case "instr":
                        return InStr(parameters);
                    case "instrrev":
                        return InStrRev(parameters);
                    case "lcase":
                        return Lcase(parameters);
                    case "left":
                        return Left(parameters);
                    case "len":
                        return Len(parameters);
                    case "lset":
                        return LSet(parameters);
                    case "ltrim":
                        return LTrim(parameters);
                    case "join":
                        return join(parameters);
                    case "mid":
                        return Mid(parameters);
                    case "replace":
                        return Replace(parameters);
                    case "right":
                        return Right(parameters);
                    case "rset":
                        return RSet(parameters);
                    case "rtrim":
                        return RTrim(parameters);
                    case "space":
                        return "".PadLeft(Convert.ToInt32(parameters[0]));
                    case "split":
                        return Split(parameters);
                    case "strcomp":
                        return parameters[0].ToString().CompareTo(parameters[1].ToString());
                    case "strconv":
                        return StrConv(parameters[0].ToString(), parameters[1]);
                    case "strdup":
                        return StrDup(parameters);
                    case "strreverse":
                        return StrReverse(parameters);
                    case "trim":
                        return parameters[0].ToString().Trim();
                    case "ucase":
                        return parameters[0].ToString().ToUpper();

                    //financial fucntions
                    case "ddb":
                        return DDB(parameters);
                    case "fv":
                        return FV(parameters);
                    case "ipmt":
                        return IPmt(parameters);
                    case "nper":
                        return Nper(parameters);
                    case "pmt":
                        return Pmt(parameters);
                    case "ppmt":
                        return PPmt(parameters);
                    case "pv":
                        return PV(parameters);
                    case "rate":
                        return rate(parameters);
                    case "sln":
                        return SLN(parameters);
                    case "syd":
                        return SYD(parameters);

                    //date and time 
                    case "cdate":
                        return Convert.ToDateTime(parameters[0], CultureInfo.CurrentCulture);
                    case "dateadd":
                        return DateAdd(parameters);
                    case "datediff":
                        return DateDiff(parameters);
                    case "datepart":
                        return DatePart(parameters);
                    case "dateserial":
                        return DateSerial(parameters);
                    case "datestring":
                        return DateTime.Now.ToShortDateString();
                    case "datevalue":
                        return DateValue(parameters);
                    case "day":
                        return Convert.ToDateTime(parameters[0]).Day;
                    /// FormatDateTime available in Text functions.
                    case "hour":
                        return Convert.ToDateTime(parameters[0]).Hour;
                    case "minute":
                        return Convert.ToDateTime(parameters[0]).Minute;
                    case "month":
                        return Convert.ToDateTime(parameters[0]).Month;
                    case "monthname":
                        return (new DateTime(2000, Convert.ToInt32(parameters[0]), 1)).ToString("MMMM");
                    case "now":
                        return DateTime.Now;
                    case "second":
                        return Convert.ToDateTime(parameters[0]).Second;
                    case "timeofday":
                        return (new DateTime()).TimeOfDay.ToString();
                    case "timer":
                        return Timer();
                    case "timeserial":
                        return TimeSerial(parameters);
                    case "timestring":
                        return DateTime.Now.ToShortTimeString();
                    case "timevalue":
                        return Convert.ToDateTime(parameters[0]).TimeOfDay;
                    case "today":
                        return DateTime.Today;
                    case "weekday":
                        return Weekday(parameters);
                    case "weekdayname":
                        return WeekdayName(parameters);
                    case "year":
                        return Convert.ToDateTime(parameters[0]).Year;

                    //inspection
                    case "isarray":
                        return (parameters[0] is IEnumerable);
                    case "isdate":
                        return IsDate(parameters);
                    case "isnumeric":
                        return IsNumeric(parameters[0].ToString());
                    case "isnothing":
                        return IsNothing(parameters);

                    // Program flow
                    case "choose":
                        return Choose(parameters);
                    case "iif":
                        return Iif(parameters);
                    case "switch":
                        return Case(parameters);

                    //conversion
                    case "cbool":
                        return Convert.ToInt32(parameters[0]) != 0 ? true : false;
                    case "cbyte":
                        return cbyte(parameters);
                    case "cchar":
                        return cchar(parameters);
                    /// CDate available in Data&Time
                    case "cdbl":
                        return Convert.ToDouble(parameters[0], CultureInfo.CurrentCulture);
                    case "cdec":
                        return Convert.ToDecimal(parameters[0], CultureInfo.CurrentCulture);
                    case "cint":
                        return Convert.ToInt32(parameters[0], CultureInfo.CurrentCulture);
                    case "clng":
                        return clng(parameters);
                    case "cobj":
                        return parameters[0];
                    case "cshort":
                        return Convert.ToInt16(parameters[0]);
                    case "csng":
                        return Convert.ToSingle(parameters[0], CultureInfo.CurrentCulture);
                    case "cstr":
                        return cstr(parameters);
                    /// Fix available in math fucntions.
                    case "hex":
                        return Convert.ToString(Convert.ToInt32(parameters[0]), 16).ToUpper();
                    /// Int available in math functions.
                    case "oct":
                        return Convert.ToString(Convert.ToInt32(parameters[0]), 8);
                    case "val":
                        return Val(parameters);

                    case "cuint":
                        return Convert.ToUInt32(parameters[0], CultureInfo.CurrentCulture);
                    case "culng":
                        return Convert.ToUInt64(parameters[0], CultureInfo.CurrentCulture);
                    case "str":
                        return parameters[0].ToString();

                    /// miscellellaneous
                    case "rownumber":
                        if (parameters[0] != null)
                        {
                            return this.expressionEngine.RowNumbers.Count != 0 ? this.expressionEngine.RowNumbers[parameters[0].ToString()] : 0;
                        }
                        else
                        {
                            return this.expressionEngine.RowNumbers.Count != 0 ? (int) this.expressionEngine.RowNumbers.First().Value : 0;
                        }

                    default:
                        return "#Error";
                }
            }

            public static bool IsNumeric(object value)
            {
                if (value != null)
                {
                    double result;
                    return double.TryParse(value.ToString(), out result);
                }

                return false;
            }

            private object Timer()
            {
                TimeSpan seconds = DateTime.Now - DateTime.Today;
                object obj = seconds.TotalSeconds;
                return obj;
            }

            private static object FormatDateTime(object[] parameters)
            {
                int formatValue = int.Parse(parameters[1].ToString());

                switch (formatValue)
                {
                    case 0:
                        {
                            DateTime date = Convert.ToDateTime(parameters[0]);
                            object obj = date.Date;
                            return obj;
                        }
                    case 1:
                        {
                            DateTime date = Convert.ToDateTime(parameters[0]);
                            object obj = date.ToLongDateString();
                            return obj;
                        }
                    case 2:
                        {
                            DateTime date = Convert.ToDateTime(parameters[0]);
                            object obj = date.ToShortDateString();
                            return obj;
                        }
                    case 3:
                        {
                            DateTime date = Convert.ToDateTime(parameters[0]);
                            object obj = date.ToLongTimeString();
                            return obj;
                        }
                    case 4:
                        {
                            DateTime date = Convert.ToDateTime(parameters[0]);
                            object obj = date.ToShortTimeString();
                            return obj;
                        }
                    default:
                        return "Invalidexpression";
                }
            }

            public static object cstr(params object[] parameters)
            {
                if (parameters[0] != null)
                {
                    return parameters[0].ToString();
                }

                return "#Error";
            }

            public static object clng(params object[] parameters)
            {
                long val = Convert.ToInt64(parameters[0]);
                string value = Convert.ToString(parameters[0]);

                if (val <= 2147483647 && val >= -2147483648)
                {
                    if (value == "true" || value == "True")
                        return -1;
                    else if (value == "false" || value == "False")
                        return 0;
                    else
                        return Convert.ToInt64(Math.Round(Convert.ToDouble(parameters[0])));
                }
                else if (val > 2147483647 || val < -2147483648)
                    return "Overflow (-2,147,483,648 to 2,147,483,647)";
                else
                    return "#Error";
            }

            private static object Val(params object[] parameters)
            {
                string value = parameters[0].ToString();
                string returnVal = string.Empty;
                MatchCollection collection = Regex.Matches(value, "\\d+");
                foreach (Match match in collection)
                {
                    returnVal += match.ToString();
                }
                return returnVal;
            }

            public static object cbyte(params object[] parameters)
            {
                bool boolValue;

                try
                {
                    return Convert.ToByte(parameters[0]);
                }
                catch (Exception e)
                {
                    double value;
                    string stringValue = parameters[0].ToString();
                    if (double.TryParse(stringValue, out value))
                    {
                        return Convert.ToByte(value);
                    }
                    else if (bool.TryParse(stringValue, out boolValue))
                    {
                        if (boolValue)
                        {
                            return 255;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }

                return "#Error";
            }

            public static object cchar(params object[] parameters)
            {
                string val = parameters[0].ToString().Trim();
                return val[0];
            }

            private object DateAdd(object[] parameters)
            {
                int enumValue = 0;

                try
                {
                    enumValue = int.Parse(parameters[0].ToString());
                }
                catch
                {
                    enumValue = this.GetStringEnum(parameters[0].ToString());
                }

                switch (enumValue)
                {
                    case 0:
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddYears(Convert.ToInt32(parameters[1]));
                            return obj;
                        }
                    case 1:
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddMonths(3);
                            return obj;
                        }
                    case 2:
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddMonths(Convert.ToInt32(parameters[1]));
                            return obj;
                        }
                    case 4:
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddDays(Convert.ToDouble(parameters[1]));
                            return obj;
                        }
                    case 5://
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddDays(Convert.ToDouble(parameters[1]));
                            return obj;
                        }
                    case 6://
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddDays(Convert.ToDouble(parameters[1]));
                            return obj;
                        }
                    case 7:
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddHours(Convert.ToDouble(parameters[1]));
                            return obj;
                        }
                    case 8:
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddMinutes(Convert.ToDouble(parameters[1]));
                            return obj;
                        }
                    case 9:
                        {
                            DateTime date = Convert.ToDateTime(parameters[2]);
                            object obj = date.AddSeconds(Convert.ToDouble(parameters[1]));
                            return obj;
                        }
                    default:
                        return "Invalidexpression";
                }
            }

            private object DateDiff(object[] parameters)
            {
                int enumValue = 0;

                try
                {
                    enumValue = int.Parse(parameters[0].ToString());
                }
                catch
                {
                    enumValue = this.GetStringEnum(parameters[0].ToString());
                }

                switch (enumValue)
                {

                    case 0:
                        {
                            DateTime date1 = Convert.ToDateTime(parameters[1]);
                            DateTime date2 = Convert.ToDateTime(parameters[2]);
                            return date2.Year - date1.Year;
                        }
                    case 1:
                        {
                            DateTime date1 = Convert.ToDateTime(parameters[1]);
                            DateTime date2 = Convert.ToDateTime(parameters[2]);
                            return (long)((((date2.Year - date1.Year * 4) + (date2.Month - 1) / 3)) - ((date1.Month - 1) / 3));
                        }
                    case 2:
                        {
                            DateTime date1 = Convert.ToDateTime(parameters[1]);
                            DateTime date2 = Convert.ToDateTime(parameters[2]);
                            return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));
                        }
                    case 4:
                        {
                            DateTime date1 = Convert.ToDateTime(parameters[1]);
                            DateTime date2 = Convert.ToDateTime(parameters[2]);
                            return (date2 - date1).TotalDays;
                        }
                    case 6:
                        {
                            DateTime date1 = Convert.ToDateTime(parameters[1]);
                            DateTime date2 = Convert.ToDateTime(parameters[2]);
                            return Fix((date2 - date1).Days / 7);
                        }
                    case 7:
                        {
                            DateTime date1 = Convert.ToDateTime(parameters[1]);
                            DateTime date2 = Convert.ToDateTime(parameters[2]);
                            return Fix((date2 - date1).TotalHours);
                        }
                    case 8:
                        {
                            DateTime date1 = Convert.ToDateTime(parameters[1]);
                            DateTime date2 = Convert.ToDateTime(parameters[2]);
                            return Fix((date2 - date1).TotalMinutes);
                        }

                    case 9:
                        {
                            DateTime date1 = Convert.ToDateTime(parameters[1]);
                            DateTime date2 = Convert.ToDateTime(parameters[2]);
                            return Fix((date2 - date1).TotalSeconds);
                        }
                    default:
                        return "Invalidexpression";
                }
            }

            private int GetStringEnum(string enumValue)
            {
                switch (enumValue.Trim())
                {
                    case "d":
                        return 4;
                    case "h":
                        return 7;
                    case "n":
                        return 8;
                    case "m":
                        return 2;
                    case "q":
                        return 1;
                    case "s":
                        return 9;
                    case "w":
                        return 6;
                    case "ww":
                        return 5;
                }

                return 0;
            }

            private static long Fix(double Number)
            {
                if (Number >= 0)
                {
                    return (long)Math.Floor(Number);
                }

                return (long)Math.Ceiling(Number);
            }

            private object DatePart(object[] parameters)
            {
                try
                {
                    int enumValue = int.Parse(parameters[0].ToString());

                    switch (enumValue)
                    {

                        case 0:
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Year;
                            }
                        case 2:
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Month;
                            }
                        case 4:
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.DayOfWeek;
                            }
                        case 7:
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Hour;
                            }
                        case 8:
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Minute;
                            }
                        case 9:
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Second;
                            }

                        default:
                            return "Invalidexpression";
                    }
                }
                catch
                {
                    string enumValues = (string)parameters[0];

                    switch (enumValues)
                    {
                        case "d":
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.DayOfWeek;
                            }
                        case "h":
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Hour;
                            }
                        case "n":
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Minute;
                            }
                        case "m":
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Month;
                            }
                        case "s":
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Second;
                            }
                        case "yyyy":
                            {
                                DateTime date = Convert.ToDateTime(parameters[1]);
                                return date.Year;
                            }
                        default:
                            return "Invalidexpression";
                    }
                }
            }

            public static object IsDate(params object[] parameters)
            {
                if (parameters[0] != null)
                {
                    DateTime result;
                    return DateTime.TryParse(parameters[0].ToString(), out result);
                }

                return false;
            }

            public static object DateValue(object[] parameters)
            {
                DateTime d = Convert.ToDateTime(parameters[0].ToString());
                return d.ToShortDateString();
            }

            public static object DateSerial(object[] parameters)
            {
                DateTime d = new DateTime(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]));
                return d.ToShortDateString();
            }

            public static object Weekday(params object[] parameters)
            {
                if (parameters.Length == 1)
                {
                    DateTime date = (DateTime)parameters[0];
                    return (int)date.DayOfWeek + 1;
                }
                if (parameters.Length == 2)
                {
                    DateTime dt = Convert.ToDateTime(parameters[0]);
                    int day = Convert.ToInt32(dt.DayOfWeek);
                    int n = Convert.ToInt32(parameters[1]);
                    return (((day + 1) - n + 7) % 7) + 1;
                }

                return string.Empty;
            }

            public static object WeekdayName(params object[] parameters)
            {
                int n = Convert.ToInt32(parameters[0]);

                if (n == 0)
                    return "Sunday";
                if (n == 1)
                    return "Monday";
                if (n == 2)
                    return "Tuesday";
                if (n == 3)
                    return "Wednesday";
                if (n == 4)
                    return "Thursday";
                if (n == 5)
                    return "Friday";
                if (n == 6)
                    return "Saturday";
                else
                    return string.Empty;
            }

            public static object TimeSerial(params object[] parameters)
            {
                int Hour = Convert.ToInt32(parameters[0]);
                int Minute = Convert.ToInt32(parameters[1]);
                int Second = Convert.ToInt32(parameters[2]);
                int num = checked(Hour * 60 * 60 + Minute * 60 + Second);
                if (num < 0)
                    checked { num += 86400; }
                return new DateTime(checked((long)num * 10000000L));
            }

            public static object Iif(params object[] parameters)
            {
                if (parameters.Length < 3)
                    return "Invalid Number of Parameters: iif(condition, val if true, val if false)";
                if (Convert.ToBoolean(parameters[0], CultureInfo.CurrentCulture))
                    return parameters[1];
                return parameters[2];
            }

            public static object Not(params object[] parameters)
            {
                try
                {
                    return !Convert.ToBoolean(parameters[0]);
                }
                catch
                {
                    return "#error";
                }
            }

            public static object Asc(params object[] parameters)
            {
                if (parameters[0] != null)
                {
                    char value = Convert.ToChar(parameters[0]);
                    return Convert.ToInt32(value);
                }
                return string.Empty;
            }

            public static object Chr(params object[] parameters)
            {
                if (parameters[0] != null)
                {
                    int value = Convert.ToInt32(parameters[0]);
                    return (char)value;
                }
                return string.Empty;
            }

            public static object GetChar(params object[] parameters)
            {
                if (parameters.Length < 2)
                {
                    return "Invalid Number of Parameters: ";
                }
                string value = parameters[0].ToString();
                int index = Convert.ToInt32(parameters[1]);
                return value[index - 1];
            }

            public static object InStr(params object[] parameters)
            {
                if (parameters.Length < 2)
                {
                    return "Invalid Number of Parameters: ";
                }

                string value = parameters[0].ToString();
                string index = parameters[1].ToString();
                return value.IndexOf(index) + 1;
            }

            public static object InStrRev(params object[] parameters)
            {
                if (parameters.Length < 2)
                {
                    return "Invalid Number of Parameters: ";
                }

                string value = parameters[0].ToString();
                string index = parameters[1].ToString();
                return value.LastIndexOf(index) + 1;
            }

            public static object Lcase(params object[] parameters)
            {
                if (parameters[0] != null)
                {
                    return parameters[0].ToString().ToLower();
                }

                return string.Empty;
            }

            public static object Left(params object[] parameters)
            {
                if (parameters.Length < 2)
                {
                    return "Invalid Number of Parameters: ";
                }

                string value = parameters[0].ToString();
                int count = Convert.ToInt32(parameters[1]);

                if (value.Length < count)
                {
                    return " Second parameter should be less than string length";
                }

                return value.Substring(0, count);
            }

            public static object Len(params object[] parameters)
            {
                if (parameters[0] != null)
                {
                    return parameters[0].ToString().Length;
                }

                return string.Empty;
            }

            public static object LSet(params object[] parameters)
            {
                if (parameters.Length < 2)
                {
                    return "Invalid Number of Parameters: ";
                }
                string value = parameters[0].ToString();
                int count = Convert.ToInt32(parameters[1]);
                if (value.Length > count)
                {
                    return value.Substring(0, count);
                }
                else
                {
                    StringBuilder sb = new StringBuilder(value);
                    while (count > sb.Length)
                    {
                        sb.Append(' ');
                    }
                    return sb;
                }
            }

            public static object LTrim(params object[] parameters)
            {
                if (parameters[0] != null)
                {
                    return parameters[0].ToString().TrimStart();
                }

                return string.Empty;
            }

            public static object Mid(params object[] parameters)
            {
                if (parameters.Length < 3)
                {
                    return "Invalid Number of Parameters: ";
                }

                string value = parameters[0].ToString();
                int start = Convert.ToInt32(parameters[1]);
                int end = Convert.ToInt32(parameters[2]);
                if (value.Length < end || value.Length < start)
                {
                    return " Second and Third parameter should be less than string length";
                }

                return value.Substring(start - 1, end);
            }

            public static object Replace(params object[] parameters)
            {
                if (parameters.Length < 3)
                {
                    return "Invalid Number of Parameters: ";
                }

                string value = parameters[0].ToString();
                string oldstring = parameters[1].ToString();
                string newstring = parameters[2].ToString();
                return value.Replace(oldstring, newstring);
            }

            public static object Right(params object[] parameters)
            {
                if (parameters.Length < 2)
                {
                    return "Invalid Number of Parameters: ";
                }

                string value = parameters[0].ToString();
                int count = Convert.ToInt32(parameters[1]);
                if (value.Length < count)
                {
                    return " Second parameter should be less than string length";
                }
                return value.Substring(value.Length - count);
            }

            public static object RSet(params object[] parameters)
            {
                if (parameters.Length < 2)
                {
                    return "Invalid Number of Parameters: ";
                }
                string value = parameters[0].ToString();
                int count = Convert.ToInt32(parameters[1]);
                if (value.Length > count)
                    return value.Substring(value.Length - count);
                else
                {
                    StringBuilder sb = new StringBuilder();
                    while (count > sb.Length)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(value);
                    return sb;
                }
            }

            public static object RTrim(params object[] parameters)
            {
                if (parameters[0] != null)
                {
                    return parameters[0].ToString().TrimEnd();
                }
                return string.Empty;
            }

            public static object StrConv(params object[] parameters)
            {
                if (Convert.ToInt32(parameters[1]) == 0)
                {
                    return parameters[0].ToString();
                }

                if (Convert.ToInt32(parameters[1]) == 1)
                {
                    return parameters[0].ToString().ToUpper();
                }

                if (Convert.ToInt32(parameters[1]) == 2)
                {
                    return parameters[0].ToString().ToLower();
                }

                if (Convert.ToInt32(parameters[1]) == 3)
                {
                    StringBuilder sb = new StringBuilder();
                    string[] values = parameters[0].ToString().Split(' ');
                    foreach (string s in values)
                    {
                        string firstStr = s.ToString()[0].ToString().ToUpper();
                        string secString = s.ToString().Substring(1).ToLower();
                        sb.Append(firstStr + secString + " ");
                    }

                    return sb;
                }

                return string.Empty;
            }

            public static object[] Split(params object[] parameters)
            {
                string value = parameters[0].ToString();
                char splitchar = ' ';

                if (parameters.Length > 1)
                {
                    splitchar = Convert.ToChar(parameters[1]);
                }

                return value.Split(splitchar);
            }

            public static object[] Filter(params object[] parameters)
            {
                object value = parameters[0];
                bool b = Convert.ToBoolean(parameters[2]);
                List<object> values = value as List<object>;
                object[] values1 = new object[values.Count];
                List<object> values2 = new List<object>();
                List<object> values3 = new List<object>();
                int i = 0;
                foreach (object o in values)
                {
                    values1[i] = o;
                    if (values[i].ToString().Contains(parameters[3].ToString()))
                        values2.Add(values1[i]);
                    else
                        values3.Add(values1[i]);
                    i++;
                }
                object[] con_true = new object[values2.Count];
                object[] con_false = new object[values3.Count];
                i = 0;
                if (b == true)
                {
                    foreach (object o in values2)
                    {
                        con_true[i] = 0;
                        i++;
                    }
                    return con_true;
                }
                else
                {
                    i = 0;
                    foreach (object o in values3)
                    {
                        con_false[i] = 0;
                        i++;
                    }
                    return con_false;
                }
            }

            public static object IsNothing(params object[] parameters)
            {
                if (parameters.Length > 0 && parameters[0] == null)
                    return true;
                else
                    return false;
            }

            public static object Case(params object[] parameters)
            {
                if (parameters.Length % 2 != 0 && parameters.Length > 0)
                    return "Invalid Number of Parameters: case(condition, val, condition2, val2, ...)";

                for (int x = 0; x < parameters.Length; x += 2)
                {
                    if ("" + parameters[x] == "else")
                        return parameters[x + 1];
                    if (Convert.ToBoolean(parameters[x], CultureInfo.CurrentCulture))
                        return parameters[x + 1];
                }
                return null;
            }

            public static object Choose(params object[] parameters)
            {
                try
                {
                    if (Convert.ToInt32(parameters[0]) > parameters.Length)
                        return "Parameters out of range";
                    else
                        return parameters[Convert.ToInt32(parameters[0])];
                }
                catch
                {
                    return null;
                }
            }

            public static object StrReverse(params object[] parameters)
            {
                char[] arr = parameters[0].ToString().ToCharArray();
                Array.Reverse(arr);
                return new string(arr);
            }

            public static object StrDup(params object[] parameters)
            {
                char a = Convert.ToChar(parameters[1]);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Convert.ToInt32(parameters[0]); i++)
                {
                    sb.Append(a);
                }
                return sb;
            }

            public static object join(params object[] parameters)
            {
                object value = parameters[0];
                List<string> stringValues = new List<string>();

                if (value is IEnumerable)
                {
                    IEnumerable values = value as IEnumerable;
                    foreach (object o in values)
                    {
                        stringValues.Add(o.ToString());
                    }
                }
                else
                {
                    stringValues.Add(value.ToString());
                }

                return string.Join(parameters[1].ToString(), stringValues.ToArray());
            }

            public static object FormatCurrency(params object[] parameters)
            {
                double value = Convert.ToDouble(parameters[0]);

                NumberFormatInfo info = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;

                if (parameters.Length > 1)
                {
                    int n = Convert.ToInt32(parameters[1]);
                    info.CurrencyDecimalDigits = n;
                }

                return value.ToString("C", info);
            }

            public static object FormatPercentage(params object[] parameters)
            {
                double value = Convert.ToDouble(parameters[0]);

                NumberFormatInfo info = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;

                if (parameters.Length > 1)
                {
                    int n = Convert.ToInt32(parameters[1]);
                    info.PercentDecimalDigits = n;
                }

                return value.ToString("P", info);
            }

            public static object Format(params object[] parameters)
            {
                if (parameters.Length == 1 || (parameters.Length == 2 && parameters[1].ToString().ToLower().Equals("standard")) || parameters[0] is string)
                {
                    return parameters[0].ToString();
                }

                if (parameters[0] is DateTime)
                {
                    DateTime dt = (DateTime)parameters[0];

                    object[] para = new object[2];
                    para[0] = dt;

                    string paramValue = parameters[1].ToString();

                    if (paramValue.Equals("GeneralDate"))
                    {
                        para[1] = 0;
                        return FormatDateTime(para);
                    }
                    else if (paramValue.Equals("LongDate"))
                    {
                        para[1] = 1;
                        return FormatDateTime(para);
                    }
                    else if (paramValue.Equals("ShortDate"))
                    {
                        para[1] = 2;
                        return FormatDateTime(para);
                    }
                    else if (paramValue.Equals("ShortTime"))
                    {
                        para[1] = 4;
                        return FormatDateTime(para);
                    }
                    else if (paramValue.Equals("LongTime"))
                    {
                        para[1] = 3;
                        return FormatDateTime(para);
                    }

                    return dt.ToString(paramValue);
                }

                double value;

                if (double.TryParse(parameters[0].ToString(), out value))
                {
                    return value.ToString(parameters[1].ToString());
                }

                return "#error";
            }

            public static object FormatNumber(params object[] parameters)
            {
                double value = Convert.ToDouble(parameters[0]);

                NumberFormatInfo info = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;

                if (parameters.Length > 1)
                {
                    int n = Convert.ToInt32(parameters[1]);

                    if (n > 1)
                    {
                        info.NumberDecimalDigits = n;
                    }
                }

                return value.ToString("N", info);
            }


            public double IPmt(params object[] parameters)
            {
                double rate = Convert.ToDouble(parameters[0]);
                int paymentPeriod = Convert.ToInt32(parameters[1]);
                int numberOfPayments = Convert.ToInt32(parameters[2]);
                double amount = Convert.ToDouble(parameters[3]);
                if (paymentPeriod < 1 || paymentPeriod > numberOfPayments)
                    throw new ArgumentException("paymentPeriod must be between 1 and numberOfPayments");
                if (rate == 0)
                    throw new ArgumentException("This implementation doesn't handle zero interest rate");

                double payment = Pmt(rate, numberOfPayments, amount);
                double futureValue = FV(rate, (paymentPeriod - 1), payment, amount);
                return (futureValue * rate);
            }

            public double PPmt(params object[] parameters)
            {
                double rate = Convert.ToDouble(parameters[0]);
                double per = Convert.ToDouble(parameters[1]);
                double nper = Convert.ToDouble(parameters[2]);
                double pv = Convert.ToDouble(parameters[3]);
                double fv = Convert.ToDouble(parameters[4]);
                double num1 = 1.0 + rate;
                double num2 = Math.Pow(rate + 1.0, nper);
                double val = (-fv - pv * num2) / (num1 * (num2 - 1.0)) * rate;
                return val - IPmt(rate, per, nper, pv, fv);
            }

            public double Pmt(params object[] parameters)
            {
                double rate = Convert.ToDouble(parameters[0]);
                int numberOfPayments = Convert.ToInt32(parameters[1]);
                double amount = Convert.ToDouble(parameters[2]);
                double temp = System.Math.Pow((rate + 1), numberOfPayments);
                return ((-amount * temp) / ((temp - 1)) * rate);
            }

            public double FV(params object[] parameters)
            {
                double rate = Convert.ToDouble(parameters[0]);
                int numberOfPayments = Convert.ToInt32(parameters[1]);
                double payment = Convert.ToDouble(parameters[2]);
                double amount = Convert.ToDouble(parameters[3]);
                double temp = System.Math.Pow(rate + 1, numberOfPayments);

                return ((-amount) * temp) - ((payment / rate) * (temp - 1));
            }

            public double PV(params object[] parameters)
            {
                double rate = Convert.ToDouble(parameters[0]);
                int npr = Convert.ToInt32(parameters[1]);
                double pmt = Convert.ToDouble(parameters[2]);
                double fv = Convert.ToDouble(parameters[3]);
                int type = Convert.ToInt32(parameters[4]);
                double val1 = 1.0;
                double val2 = Math.Pow(1.0 + rate, npr);
                return -(fv + pmt * val1 * ((val2 - 1.0) / rate)) / val2;
            }

            public double SLN(params object[] parameters)
            {
                double cost = Convert.ToDouble(parameters[0]);
                double salvage = Convert.ToDouble(parameters[1]);
                double life = Convert.ToDouble(parameters[2]);
                return (cost - salvage) / life;
            }

            public double SYD(params object[] parameters)
            {
                double cost = Convert.ToDouble(parameters[0]);
                double salvage = Convert.ToDouble(parameters[1]);
                double Life = Convert.ToDouble(parameters[2]);
                double period = Convert.ToDouble(parameters[3]);
                double temp1 = (cost - salvage) * (Life - period + 1) * 2;
                double temp2 = Life * (Life + 1);
                return temp1 / temp2;
            }

            public double DDB(params object[] parameters)
            {
                double cost = Convert.ToDouble(parameters[0]);
                double salvage = Convert.ToDouble(parameters[1]);
                double life = Convert.ToDouble(parameters[2]);
                double period = Convert.ToDouble(parameters[3]);
                double factor = Convert.ToDouble(parameters[4]);
                if (period <= 1.0)
                {
                    double val1 = cost * factor / life;
                    double val2 = cost - salvage;
                    if (val1 > val2)
                        return val2;
                    else
                        return val1;
                }
                else
                {
                    double a = (life - factor) / life;
                    double b = period - 1.0;
                    double val1 = factor * cost / life * Math.Pow(a, b);
                    return val1 < 0.0 ? 0.0 : val1;
                }
            }

            public double Nper(params object[] parameters)
            {
                double rate = Convert.ToDouble(parameters[0]);
                double pmt = Convert.ToDouble(parameters[1]);
                double pv = Convert.ToDouble(parameters[2]);
                double fv = Convert.ToDouble(parameters[3]);
                int type = Convert.ToInt32(parameters[4]);
                double num = pmt / rate;
                double d1 = -fv + num;
                double d2 = pv + num;
                double d3 = rate + 1.0;
                return (Math.Log(d1) - Math.Log(d2)) / Math.Log(d3);
            }

            public double rate(params object[] parameters)
            {
                double NPer = Convert.ToDouble(parameters[0]);
                double Pmt = Convert.ToDouble(parameters[1]);
                double PV = Convert.ToDouble(parameters[2]);
                double FV = Convert.ToDouble(parameters[3]);
                int Due = Convert.ToInt32(parameters[4]);
                double Guess = Convert.ToInt32(parameters[5]);
                double Rate1 = Guess;
                double num1 = LEvalRate(Rate1, NPer, Pmt, PV, FV, Due);
                double Rate2 = num1 <= 0.0 ? Rate1 * 2.0 : Rate1 / 2.0;
                double num2 = LEvalRate(Rate2, NPer, Pmt, PV, FV, Due);
                int num3 = 0;
                do
                {
                    if (num2 == num1)
                    {
                        if (Rate2 > Rate1)
                            Rate1 -= 1E-05;
                        else
                            Rate1 -= -1E-05;
                        num1 = LEvalRate(Rate1, NPer, Pmt, PV, FV, Due);
                    }
                    double Rate3 = Rate2 - (Rate2 - Rate1) * num2 / (num2 - num1);
                    double num4 = LEvalRate(Rate3, NPer, Pmt, PV, FV, Due);
                    if (Math.Abs(num4) < 1E-07)
                        return Rate3;
                    double num5 = num4;
                    num1 = num2;
                    num2 = num5;
                    double num6 = Rate3;
                    Rate1 = Rate2;
                    Rate2 = num6;
                    checked { ++num3; }
                }
                while (num3 <= 39);
                return 0;
            }

            public static double LEvalRate(double Rate, double NPer, double Pmt, double PV, double dFv, int Due)
            {
                if (Rate == 0.0)
                    return PV + Pmt * NPer + dFv;
                double num1 = Math.Pow(Rate + 1.0, NPer);
                double num2 = Due == 0 ? 1.0 : 1.0 + Rate;
                return PV * num1 + Pmt * num2 * (num1 - 1.0) / Rate + dFv;
            }
            #endregion
        }

        internal class ExpressionMatch
        {
            private const string NumericMatch = @"(?:[0-9]+)?(?:\.[0-9]+)?(?:E-?[0-9]+)?(?=\b)";
            private const string BoolMatch = @"true|false";
            private const string FunctionMatch = @"(?<Function>\w+)\s*\(";
            private const string ObjectFunctionMatch = @".(?<Function>\w+)\(";
            private const string CodeFunctionMatch = @"(\w+\.)+(?<Function>\w+)\(";
            private const string StringMatch = "\\\"\\\"|\\\"(?<String>.*?[^\\\\])\\\"|\"(?<String>.*?[^\\\\])\"|“(?<String>.*?[^\\\\])”";
            private const string ParameterMatch = @"Parameters!\s*(?<Parameter>\w+)\s*.(?<Data>Value|Label)";
            private const string FieldMatch = @"Fields!\s*(?<Field>\w+)\s*.Value";
            private const string EnumMatch = @"(?<Enum>(CompareMethod|DueDate|vbStrConv|FirstDayOfWeek|DateFormat|DateInterval)).(?<Value>\w+)";
            private const string ConstMatch = @"(?<Name>(Now|Nothing))";

            internal static string AndOperator = "&&";
            internal static string OrOperator = "||";
            internal static string ModOperator = "%";
            internal static string LikeOperator = "@1";
            internal static string IsOperator = "@2";
            internal static string XorOperator = "@3";

            //private const string logicalMatch = @"\\|(?i:\s*And\s*)|(?i:\s*Or\s*)|(?i:\s*Mod\s*)|(?i:\s*Like\s*)|(?i:\s*OrElse\s*)|(?i:\s*AndAlso\s*)|(?i:\s*Xor\s*)|(?i:\s*Is\s*)";
            private const string logicalMatch = @"\\|\w+";

            private static string UnaryMatch = @"(?:\+|-|!|~)(?=\w|\()";
            private static string BinaryOpMatch = @"<<|>>|\+|-|\*|/|%|&&|\|\||&|\||\^|==|!=|>=|<=|=|<|>|(" + LikeOperator + ")|(" + IsOperator + ")|(" + XorOperator + ")";
            private static string DateMatch = @"\d{1,2}[-/]\d{1,2}[-/](?:\d{4}|\d{2})" +
                                                      @"(?:\s+\d{1,2}\:\d{2}(?:\:\d{2})?\s*(?:AM|PM)?)?" + "|(#(?<Date>.*?[^\\\\])#)";
            private static string DateMatch2 = @"(#(?<Date>.*?[^\\\\])#)";
            private const string TimeSpanmatch = @"(?:(?<Days>\d+)\.|)(?<Hours>\d{1,2})\:(?<Minutes>\d{1,2})" +
                                                      @"(?:\:(?<Seconds>\d{1,3})(?:\.(?<Milliseconds>\d{1,3})|)|)";
            private const string WhitSpaceMatch = @"\s+";

            internal static Regex Numeric = new Regex(
                NumericMatch,
                RegexOptions.None
            );


            internal static Regex Boolean = new Regex(
                BoolMatch,
                RegexOptions.None | RegexOptions.IgnoreCase
            );

            internal static Regex UnaryOp = new Regex(
                @"(?<=(?:" + BinaryOpMatch + @")\s*|\A)(?:" + UnaryMatch + @")",
                RegexOptions.None
            );

            internal static Regex BinaryOp = new Regex(
                @"(?<!(?:" + BinaryOpMatch + @")\s*|^\A)(?:" + BinaryOpMatch + @")",
                RegexOptions.None
            );

            internal static Regex Parenthesis = new Regex(
                @"\(",
                RegexOptions.None
            );

            internal static Regex Function = new Regex(
                FunctionMatch,
                RegexOptions.None | RegexOptions.IgnoreCase
            );

            internal static Regex ObjectFunction = new Regex(
                ObjectFunctionMatch,
                RegexOptions.None
            );

            internal static Regex CodeMatchFunction = new Regex(
                CodeFunctionMatch,
                RegexOptions.None
            );

            internal static Regex DateTime = new Regex(
                @"@dt\((?<DateString>" + DateMatch + @")\)",
                RegexOptions.None | RegexOptions.IgnoreCase
            );

            internal static Regex DateTime2 = new Regex(
                DateMatch2,
                RegexOptions.None | RegexOptions.IgnoreCase
            );

            internal static Regex TimeSpan = new Regex(
                @"@ts\(" + TimeSpanmatch + @"\)",
                RegexOptions.None
            );

            internal static Regex String = new Regex(
                StringMatch,
                RegexOptions.None
            );

            internal static Regex Parameter = new Regex(
                ParameterMatch,
                RegexOptions.None
            );

            internal static Regex Field = new Regex(
            FieldMatch,
            RegexOptions.None
            );

            internal static Regex WhiteSpace = new Regex(
                WhitSpaceMatch,
                RegexOptions.None
            );

            internal static Regex Enum = new Regex(
                EnumMatch,
                RegexOptions.None | RegexOptions.IgnoreCase
            );

            internal static Regex Const = new Regex(
                ConstMatch,
                RegexOptions.None | RegexOptions.IgnoreCase
            );

            internal static Regex LogicalOperator = new Regex(
                logicalMatch,
                RegexOptions.None
            );

            internal static Regex ExecutionTimeMatch = new Regex(
               "Globals!ExecutionTime",
               RegexOptions.None
           );
        }
    }
}
