//-------------------------------------------------------------------------------------------------
// <copyright file="Diagnostics.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

namespace Syncfusion.Grouping
{
    /// <summary>Internal only</summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class Diagnostics
    {
        /// <internalonly/>
        public static int MaxLineLength = 120;

        /// <internalonly/>
        public static int MaxCount = 100;

        /// <summary>Internal only.</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void DumpElement(Element el)
        {
            Table pt = el.ParentTable;
            while (pt.RelationParentTable != null)
            {
                pt = pt.RelationParentTable;
            }

            string s = el.ToString();
            int nestedIndex = pt.NestedDisplayElements.IndexOf(el);
            int nestedGroupedIndex = pt.NestedElements.IndexOf(el);
            string t = nestedGroupedIndex.ToString() + "/" + nestedIndex.ToString() + "/" + el.ParentTable.DisplayElements.IndexOf(el).ToString() + ": " + s;
            if (t.Length > MaxLineLength)
            {
                t = t.Substring(0, MaxLineLength - 4) + " ...";
            }

            Console.WriteLine(t);
        }

        /// <summary>Internal only.</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughDisplayElement(Table table)
        {
            Console.WriteLine("DisplayElements");
            Console.WriteLine("---------------");
            int i = 0;
            i = 0;
            foreach (Element el in table.DisplayElements)
            {
                DumpElement(el);
                if (i++ > MaxCount)
                {
                    break;
                }
            }

            Console.WriteLine(String.Format("DisplayElements.Count = {0}", table.DisplayElements.Count));
            Console.WriteLine(string.Empty);
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughNestedDisplayElement(Table table)
        {
            Console.WriteLine("NestedDisplayElements");
            Console.WriteLine("---------------------");
            int i = 0;
            i = 0;
            foreach (Element el in table.NestedDisplayElements)
            {
                DumpElement(el);
                if (i++ > MaxCount)
                {
                    break;
                }
            }
            ////            DumpElement(table.NestedDisplayElements[3]);
            ////            DumpElement(table.NestedDisplayElements[4]);
            ////            DumpElement(table.NestedDisplayElements[31]);
            ////            DumpElement(table.NestedDisplayElements[33]);
            i = 0;
            for (int k = table.NestedDisplayElements.Count; k > 0; k--)
            {
                Element el = table.NestedDisplayElements[k - 1];
                DumpElement(el);
                if (i++ > MaxCount)
                {
                    break;
                }
            }

            Console.WriteLine(String.Format("NestedDisplayElements.Count = {0}", table.NestedDisplayElements.Count));
            Console.WriteLine(string.Empty);
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughAllElement(Table table)
        {
            Console.WriteLine("Elements");
            Console.WriteLine("--------");

            ////            for (int i = 0; i < table.Elements.Count; i++)
            ////
            int i = 0;
            i = 0;
            foreach (Element el in table.Elements)
            {
                DumpElement(el);
                if (i++ > MaxCount)
                {
                    break;
                }
            }

            Console.WriteLine(String.Format("Elements.Count = {0}", table.Elements.Count));
            Console.WriteLine(string.Empty);
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughNestedElements(Table table)
        {
            Console.WriteLine("NestedElements");
            Console.WriteLine("--------------");

            ////            for (int i = 0; i < table.Elements.Count; i++)
            ////
            int i = 0;
            i = 0;
            foreach (Element el in table.NestedElements)
            {
                DumpElement(el);
                if (i++ > MaxCount)
                {
                    break;
                }
            }

            ////            DumpElement(table.NestedElements[2]);
            ////            DumpElement(table.NestedElements[1]);
            ////            DumpElement(table.NestedElements[3]);
            ////            DumpElement(table.NestedElements[4]);
            ////            i = 0;
            ////            for (int k = 0; k < table.NestedElements.Count && k < MaxCount; k++ )
            ////            {
            ////                Element el = table.NestedElements[k];
            ////                if (k == 53)
            ////                    DumpElement(el);
            ////                DumpElement(el);
            ////            }
            ////
            ////            for (int k = table.NestedElements.Count; k > 0; k--)
            ////            {
            ////                Element el = table.NestedElements[k-1];
            ////                DumpElement(el);
            ////                if (i++ > MaxCount)
            ////                    break;
            ////            }

            Console.WriteLine(String.Format("Elements.Count = {0}", table.Elements.Count));
            Console.WriteLine(string.Empty);
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughUnsortedRecords(Table table)
        {
            Console.WriteLine("UnsortedRecords");
            Console.WriteLine("---------------");
            int i = 0;
            i = 0;
            foreach (Record el in table.UnsortedRecords)
            {
                DumpElement(el);
                if (i++ > MaxCount)
                {
                    break;
                }
            }

            Console.WriteLine(String.Format("UnsortedRecords.Count = {0}", table.UnsortedRecords.Count));
            Console.WriteLine(string.Empty);
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughAllSortedRecords(Table table)
        {
            Console.WriteLine("SortedRecords");
            Console.WriteLine("-------------");
            int i = 0;
            i = 0;
            foreach (Record el in table.Records)
            {
                DumpElement(el);
                if (i++ > MaxCount)
                {
                    break;
                }
            }

            Console.WriteLine(String.Format("SortedRecords.Count = {0}", table.Records.Count));
            Console.WriteLine(string.Empty);
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughFilteredRecords(Table table)
        {
            Console.WriteLine("FilteredRecords");
            Console.WriteLine("---------------");
            int i = 0;
            i = 0;
            foreach (Record el in table.FilteredRecords)
            {
                DumpElement(el);
                if (i++ > MaxCount)
                {
                    break;
                }
            }

            Console.WriteLine(String.Format("FilteredRecords.Count = {0}", table.FilteredRecords.Count));
            Console.WriteLine(string.Empty);
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughGroups(Table table)
        {
            Console.WriteLine("Groups");
            Console.WriteLine("------");
            int i = 0;
            i = 0;
            foreach (Group group in table.TopLevelGroup.Groups)
            {
                string t = group.Category != null ? group.Category.ToString() : "(null)";
                Console.WriteLine(t);
                IterateThroughGroup(t, group);
                if (i++ > MaxCount)
                {
                    break;
                }
            }

            Console.WriteLine(String.Format("TopLevelGroup.Groups.Count = {0}", table.FilteredRecords.Count));
            Console.WriteLine(string.Empty);
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void IterateThroughGroup(string s, Group parentGroup)
        {
            int i = 0;
            i = 0;
            foreach (Group group in parentGroup.Groups)
            {
                string t = s + " : " + (group.Category != null ? group.Category : "(null)");
                Console.WriteLine(t);
                IterateThroughGroup(t, group);
                if (i++ > MaxCount)
                {
                    break;
                }
            }
        }

        /// <summary>Internal only</summary>
        /// <internalonly/>
        [Conditional("DEBUG")]
        public static void DumpSummaries(Element el)
        {
            Console.WriteLine("Summaries for " + el.ToString());
            int i = 0;

            ISummary[] sums = el.GetSummaries(el.ParentTable);
            Table table = el.ParentTable;
            if (el is Table)
            {
                table = (Table)el;
            }

            if (sums != null && table != null)
            {
                foreach (ISummary sum in sums)
                {
                    Console.WriteLine(table.TableDescriptor.Summaries[i++].Name + ": " + sum.ToString());
                }
            }

            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
        }
    }
}
