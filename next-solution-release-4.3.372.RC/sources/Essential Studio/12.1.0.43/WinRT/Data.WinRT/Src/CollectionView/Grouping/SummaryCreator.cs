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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Syncfusion.Data.Extensions;
#if WPF
using System.Data;
using System.Windows.Data;
#if !SyncfusionFramework3_5
using System.Threading.Tasks;
#endif
#elif WinRT
using System.Threading.Tasks;
#elif SILVERLIGHT || WP
using System.Windows.Data;
#endif

namespace Syncfusion.Data
{
    public static class SummaryCreator
    {
        public static string ColumnName = "ColumnName";

        public static string Key = "Key";

        public static string ItemsCount = "ItemsCount";

        public static string GetSummaryDisplayTextForRow(SummaryRecordEntry summaryEntry, ICollectionViewAdv collectionView, string columnHeaderName = null)
        {
            var summaryItems = summaryEntry.SummaryValues;
            var summaryRow = summaryEntry.SummaryRow;
            var rowValues = new Dictionary<string, object>();
            summaryRow.SummaryColumns.ForEach(col =>
            {
                var item = summaryItems.FirstOrDefault(s => s.Name == col.Name);
                var value = GetFormattedSummary(item.AggregateValues, col, collectionView);
                rowValues.Add(col.Name, value);
            });

            if (summaryEntry.Parent != null && summaryEntry.Parent is Group)
            {
                var group = summaryEntry.Parent as Group;
                if (summaryRow.Title.Contains("{Key}"))
                    rowValues.Add("Key", group.Key);
                if (summaryRow.Title.Contains("{ColumnName}") && !string.IsNullOrEmpty(columnHeaderName))
                    rowValues.Add("ColumnName", columnHeaderName);
            }

            var result = summaryRow.Title.FormatByName(rowValues);
            return result;
        }

        public static string GetSummaryDisplayText(SummaryRecordEntry summaryEntry, string columnName, ICollectionViewAdv collectionView)
        {
            string result = string.Empty;
            var summaryRow = summaryEntry.SummaryRow;
            var summaryCol = summaryRow.SummaryColumns.FirstOrDefault(s => s.MappingName == columnName);
            var summaryItems = summaryEntry.SummaryValues;
            if (summaryItems != null && summaryCol != null)
            {
                var item = summaryItems.FirstOrDefault(s => s.Name == summaryCol.Name);
                if (item != null)
                    result = SummaryCreator.GetFormattedSummary(item.AggregateValues, summaryCol, collectionView);
            }
            return result;
        }

        public static string GetSummaryDisplayText(SummaryRecordEntry summaryEntry, string columnName, ICollectionViewAdv collectionView, Group group)
        {
            string result = string.Empty;
            var summaryRow = summaryEntry.SummaryRow;
            var summaryCol = summaryRow.SummaryColumns.FirstOrDefault(s => s.MappingName == columnName);
            var summaryItems = summaryEntry.SummaryValues;
            if (summaryItems != null && summaryCol != null)
            {
                var item = summaryItems.FirstOrDefault(s => s.Name == summaryCol.Name);
                if (item != null)
                    result = SummaryCreator.GetFormattedSummary(item.AggregateValues, summaryCol, collectionView, group);
            }
            return result;
        }

        public static object GetSummaryDisplayValue(SummaryRecordEntry summaryEntry, string columnName, string aggregateKey)
        {
            string result = string.Empty;
            var summaryRow = summaryEntry.SummaryRow;
            var summaryColumns = summaryRow.SummaryColumns.Where(s => s.MappingName == columnName).ToList();
            var summaryItems = summaryEntry.SummaryValues;
            if (summaryItems != null && summaryColumns != null)
            {
                foreach (var column in summaryColumns)
                {
                    var item = summaryItems.FirstOrDefault(s => s.Name == column.Name);
                    bool value = item.AggregateValues.ContainsKey(aggregateKey);
                    if (value)
                        return item.AggregateValues[aggregateKey];
                }
            }
            return result;
        }

        public static object RaiseQuerySummaryAggregate(IEnumerable items, ISummaryColumn summaryColumn, Dictionary<string, object> summaries, ICollectionViewAdv collectionView)
        {
            var summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, collectionView);
            var result = SummaryCreator.GetFormattedSummary(items, summaryAggregate, summaries, summaryColumn, collectionView);

            return result;
        }

        internal static object GetFormattedSummary(IEnumerable items, ISummaryAggregate summaryAggregate, Dictionary<string, object> summaries, ISummaryColumn summaryColumn, ICollectionViewAdv collectionView)
        {
            string parsedFormat = string.Empty;

            object[] objs = SummaryCreator.GetSummary(items, summaryAggregate, summaries, summaryColumn, out parsedFormat, collectionView) as object[];
            if (objs != null)
            {
                return string.Format(parsedFormat, objs);
            }
            return string.Empty;
        }

        internal static string GetFormattedSummary(Dictionary<string, object> summaryItems, ISummaryColumn summaryColumn, ICollectionViewAdv collectionView)
        {
            var summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, collectionView);
            if (summaryAggregate != null && summaryItems.Count > 0)
            {
                string parsedFormat = string.Empty;
                SummaryCreator.ParseFormat(false, summaryAggregate, summaryColumn, out parsedFormat);
                var objs = summaryItems.Values.ToArray();
                return string.Format(parsedFormat, objs);
            }

            return string.Empty;
        }

        internal static string GetFormattedSummary(Dictionary<string, object> summaryItems, ISummaryColumn summaryColumn, ICollectionViewAdv collectionView, Group group)
        {
            var summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn, collectionView);
            if (summaryAggregate != null)
            {
                string parsedFormat = string.Empty;
                SummaryCreator.ParseFormat(false, summaryAggregate, summaryColumn, out parsedFormat, collectionView, group);
                var objs = summaryItems.Values.ToArray();
                return string.Format(parsedFormat, objs);
            }

            return string.Empty;
        }


#if WPF
        internal static PropertyDescriptor[] ParseFormat(bool raiseException, ISummaryAggregate summaryAggregate, ISummaryColumn SummaryColumn, out string parsedFormat, ICollectionView collectionView, Group group)
#else
        internal static PropertyInfo[] ParseFormat(bool raiseException, ISummaryAggregate summaryAggregate, ISummaryColumn SummaryColumn, out string parsedFormat, ICollectionViewAdv collectionView, Group group)
#endif

        {
            parsedFormat = string.Empty;

#if WPF
            var pdc = TypeDescriptor.GetProperties(summaryAggregate);
            var groupProperty = TypeDescriptor.GetProperties(typeof(SortColumn)).Find(ColumnName, false);
            var keyProperty = TypeDescriptor.GetProperties(typeof(Group)).Find(Key, false);
            var countProperty = TypeDescriptor.GetProperties(typeof(Group)).Find(ItemsCount, false);
            var al = new List<PropertyDescriptor>();
#else
            var pdc = summaryAggregate.GetType().GetProperties();
            var sortProperty = typeof(SortColumn).GetProperties();
            var groupProperty = sortProperty.FirstOrDefault(p => p.Name == "ColumnName");
            var propertyInfoArray = group.GetType().GetProperties();
            var keyProperty = propertyInfoArray.FirstOrDefault(p => p.Name == "Key");
            var countProperty = propertyInfoArray.FirstOrDefault(p => p.Name == "ItemsCount");
            var al = new List<PropertyInfo>();
#endif
            int n1 = SummaryColumn.Format.IndexOf("{");
            var sb = new StringBuilder();
            int n2 = 0;
            if (n1 == -1)
            {
                sb.Append(SummaryColumn.Format);
            }
            else
            {
                sb.Append(SummaryColumn.Format.Substring(0, n1 + 1));
            }

            int count = 0;

            while (n1 != -1)
            {

                n2 = SummaryColumn.Format.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = SummaryColumn.Format.IndexOfAny(new char[] { '}', ':' }, n1);
                    string name = SummaryColumn.Format.Substring(n1 + 1, n3 - n1 - 1);
#if WPF
                    var pd = pdc[name];
#else
                    var pd = pdc.FirstOrDefault(p => p.Name == name);
#endif
                    if (pd != null)
                    {
                        sb.Append((al.Count - count).ToString());
                        al.Add(pd);
                    }
                    else
                    {
                        if (raiseException)
                            throw new FormatException("FilterPredicates not found: " + name);
                        else
                        {
#if WPF
                            if (name == ColumnName)
                            {
                                al.Add(groupProperty);
                                count++;
                            }
                            else if (name == Key)
                            {
                                al.Add(keyProperty);
                                count++;
                            }
                            else if (name == ItemsCount)
                            {
                                al.Add(countProperty);
                                count++;
                            }
#else
                            SummaryColumn.Format = string.Empty;
                            return new PropertyInfo[0];
#endif
                        }
                    }
                    n1 = SummaryColumn.Format.IndexOf("{", n2);
                    if (n1 == -1)
                    {
                        sb.Append(SummaryColumn.Format.Substring(n3));
                    }
                    else
                    {
                        sb.Append(SummaryColumn.Format.Substring(n3, n1 - n3 + 1));
                    }
                }
                else
                {
                    if (raiseException)
                    {
                        throw new FormatException("No closing char found: " + sb.ToString());
                    }
                    else
                    {
                        SummaryColumn.Format = string.Empty;
#if WPF
                        return new PropertyDescriptor[0];
#else
                        return new PropertyInfo[0];
#endif
                    }
                }

            }
            parsedFormat = sb.ToString();

            foreach (var pd in al)
            {
                if (pd.Name == ColumnName)
                {
                    int index = al.IndexOf(pd);
                    var groupedCol = collectionView.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
                    var value = groupedCol.PropertyName;
                    var rindex = parsedFormat.IndexOf("{}");
                    parsedFormat = parsedFormat.Remove(rindex, 2);
                    if (rindex != 0)
                    {
                        parsedFormat = parsedFormat.Insert(rindex - 1, value);
                    }
                    else
                    {
                        parsedFormat = parsedFormat.Insert(rindex, value);
                    }
                }

                if (pd.Name == Key)
                {
                    int index = al.IndexOf(pd);
                    var value = pd.GetValue(group);
                    var rindex = parsedFormat.IndexOf("{}");
                    parsedFormat = parsedFormat.Remove(rindex, 2);
                    parsedFormat = parsedFormat.Insert(rindex - 1, value.ToString());
                }

                if (pd.Name == ItemsCount)
                {
                    int index = al.IndexOf(pd);
                    var value = pd.GetValue(group);
                    parsedFormat = parsedFormat.Replace("{}", value.ToString());
                }
            }
            al.Remove(groupProperty);
            al.Remove(keyProperty);
            al.Remove(countProperty);

            return al.ToArray();
        }

#if WPF
        internal static PropertyDescriptor[] ParseFormat(bool raiseException, ISummaryAggregate summaryAggregate, ISummaryColumn SummaryColumn, out string parsedFormat)
#else
        internal static PropertyInfo[] ParseFormat(bool raiseException, ISummaryAggregate summaryAggregate, ISummaryColumn SummaryColumn, out string parsedFormat)
#endif
        {
            parsedFormat = string.Empty;
#if WPF
            var pdc = TypeDescriptor.GetProperties(summaryAggregate);
            var al = new List<PropertyDescriptor>();
#else
            var pdc = summaryAggregate.GetType().GetProperties();
            var al = new List<PropertyInfo>();
#endif
            string SummaryColumnFormat = SummaryColumn.Format;
            SummaryColumnFormat = SummaryColumnFormat.Replace("{ColumnName}", "");
            SummaryColumnFormat = SummaryColumnFormat.Replace("{Key}", "");
            SummaryColumnFormat = SummaryColumnFormat.Replace("{ItemsCount}", "");

            int n1 = SummaryColumnFormat.IndexOf("{");
            var sb = new StringBuilder();
            int n2 = 0;
            if (n1 == -1)
            {
                sb.Append(SummaryColumnFormat);
            }
            else
            {
                sb.Append(SummaryColumnFormat.Substring(0, n1 + 1));
            }

            while (n1 != -1)
            {
                n2 = SummaryColumnFormat.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = SummaryColumnFormat.IndexOfAny(new char[] { '}', ':' }, n1);
                    string name = SummaryColumnFormat.Substring(n1 + 1, n3 - n1 - 1);
#if WPF
                    var pd = pdc[name];
#else
                    var pd = pdc.FirstOrDefault(p => p.Name == name);
#endif
                    if (pd != null)
                    {
                        sb.Append(al.Count.ToString());
                        al.Add(pd);
                    }
                    else
                    {
                        if (raiseException)
                            throw new FormatException("FilterPredicates not found: " + name);
                        else
                        {
#if WPF
                            return new PropertyDescriptor[0];
#else
                            return new PropertyInfo[0];
#endif
                        }
                    }
                    n1 = SummaryColumnFormat.IndexOf("{", n2);
                    if (n1 == -1)
                    {
                        sb.Append(SummaryColumnFormat.Substring(n3));
                    }
                    else
                    {
                        sb.Append(SummaryColumnFormat.Substring(n3, n1 - n3 + 1));
                    }
                }
                else
                {
                    if (raiseException)
                    {
                        throw new FormatException("No closing char found: " + sb.ToString());
                    }
                    else
                    {
                        SummaryColumn.Format = string.Empty;
#if WPF
                        return new PropertyDescriptor[0];
#else
                        return new PropertyInfo[0];
#endif
                    }
                }
            }
            parsedFormat = sb.ToString();
            return al.ToArray();
        }

        internal static object GetSummary(IEnumerable items, ISummaryAggregate summaryAggregate, Dictionary<string, object> result, ISummaryColumn summaryColumn, out string parsedFormat, ICollectionViewAdv collectionView)
        {
            parsedFormat = string.Empty;
            if (summaryAggregate == null)
            {
                return null;
            }

            var enumerator = items.GetEnumerator();
            var hasValue = enumerator.MoveNext();
            var unboundExpressionFunc = collectionView as IUnboundExpressionFunc;
            var pds = SummaryCreator.ParseFormat(false, summaryAggregate, summaryColumn, out parsedFormat);
            Delegate summaryDelg = null;
            var isSummaryExp = (summaryAggregate is ISummaryExpressionAggregate) && (unboundExpressionFunc != null && unboundExpressionFunc.GetExpressionFunc(summaryColumn.MappingName) != null);
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
            var parallizableSummary = summaryAggregate as ISummaryParallelizable;
            var parallizableView = collectionView as IParallelizableView;
            if (parallizableSummary != null && parallizableView != null)
            {
                parallizableSummary.CanParallelize = parallizableView.UsePLINQ;
            }
#endif
            if (isSummaryExp)
            {
                summaryDelg = ((ISummaryExpressionAggregate)summaryAggregate).CalculateAggregateExpressionFunc();
            }
            else
            {
                summaryDelg = summaryAggregate.CalculateAggregateFunc();
            }
            object[] objs = new object[pds.Length];
            try
            {
                for (int i = 0; i < pds.Length; i++)
                {
                    if (pds[i] != null)
                    {
                        if (hasValue)
                        {
                            if (!isSummaryExp)
                            {
                                // calculates the summaries and updates the properties
                                summaryDelg.DynamicInvoke(new object[] { items, summaryColumn.MappingName, pds[i] });
                            }
                            else
                            {
                                summaryDelg.DynamicInvoke(new object[] { items, summaryColumn.MappingName, unboundExpressionFunc.GetExpressionFunc(summaryColumn.MappingName), pds[i] });
                            }
#if WPF
                            objs[i] = pds[i].GetValue(summaryAggregate);
#else
                            objs[i] = pds[i].GetValue(summaryAggregate, null);
#endif
                            if (result != null)
                            {
                                result.Add(pds[i].Name, objs[i]);
                            }
                        }
                        else
                        {
                            if (result != null)
                            {
                                if(summaryColumn.SummaryType == SummaryType.Custom && summaryColumn.CustomAggregate != null)
                                    summaryDelg.DynamicInvoke(new object[] { items, summaryColumn.MappingName, pds[i] });
                                result.Add(pds[i].Name, pds[i].GetValue(summaryAggregate)); 
                            }
                        }
                    }
                    else
                    {
                        objs[i] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return objs;
        }

        public static ISummaryAggregate GetSummaryAggregate(ISummaryColumn summaryColumn, ICollectionViewAdv collectionView)
        {
            ISummaryAggregate summaryAggregate = null;

            if (summaryColumn.SummaryType == SummaryType.Custom && summaryColumn.CustomAggregate != null)
            {
                summaryAggregate = summaryColumn.CustomAggregate;
            }
            else
            {
                summaryAggregate = SummaryCreator.GetSummaryAggregate(summaryColumn.SummaryType, collectionView);
            }

            return summaryAggregate;
        }

        private static ISummaryAggregate GetSummaryAggregate(SummaryType summaryType, ICollectionViewAdv collectionView)
        {
#if WPF
            var isLegacyDataTable = collectionView.SourceCollection is DataTable || collectionView.SourceCollection is DataView;
            if (!isLegacyDataTable)
            {
#endif
            switch (summaryType)
            {
                case SummaryType.CountAggregate:
                    return new CountAggregate();
                case SummaryType.DoubleAggregate:
                    return new DoubleAggregate();
                case SummaryType.Int32Aggregate:
                    return new Int32Aggregate();
            }
#if WPF
            }
            else
            {
                DataTable table;
                if (collectionView.SourceCollection is DataView)
                {
                    var dv = collectionView.SourceCollection as DataView;
                    table = dv.Table;
                }
                else
                    table = (collectionView.SourceCollection as DataTable);

                switch (summaryType)
                {
                    case SummaryType.CountAggregate:
                        return new DataTableCountAggregate(table);
                    case SummaryType.Int32Aggregate:
                        return new DataTableInt32Aggregate(table);
                    case SummaryType.DoubleAggregate:
                        return new DataTableDoubleAggregate(table);
                }
            }
#endif
            return null;
        }
    }
}
