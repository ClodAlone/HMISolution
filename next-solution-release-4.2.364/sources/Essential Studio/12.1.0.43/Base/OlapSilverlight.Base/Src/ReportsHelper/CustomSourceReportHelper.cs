#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using Syncfusion.OlapSilverlight.Engine;

namespace Syncfusion.OlapSilverlight.Reports
{
    internal class CustomSourceReportHelper
    {
        /// <summary>
        /// Processes the report.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <param name="olapReport">The olap report.</param>
        internal static void ProcessReport(PivotCellDescriptor cellDescriptor,GridLayout gridLayout,OlapReport olapReport)
        {
            PivotElements pivotDataElements = new PivotElements(olapReport);

            if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
            {
                foreach (var item in olapReport.SeriesElements)
                {
                    DimensionElement dimensionElement = item.ElementValue as DimensionElement;

                    if (dimensionElement != null)
                    {
                        if (dimensionElement.Hierarchy != null)
                        {
                            if (dimensionElement.Hierarchy.LevelElements[0].Name == cellDescriptor.UniqueName)
                            {
                                if (!CheckMemberDrillDown(dimensionElement.Hierarchy.LevelElements[0].MemberElements, cellDescriptor.CellValue))
                                {
                                    dimensionElement.Hierarchy.LevelElements[0].MemberElements.Add(new MemberElement { Name = cellDescriptor.CellValue, UniqueName = cellDescriptor.UniqueName + "." + cellDescriptor.CellValue });
                                }
                                else
                                {
                                    dimensionElement.Hierarchy.LevelElements[0].MemberElements.RemoveAt(GetElementIndex(dimensionElement.Hierarchy.LevelElements[0].MemberElements, cellDescriptor.CellValue));
                                }
                            }
                            else
                            {
                                DrillUpDown(dimensionElement.Hierarchy.LevelElements[0], cellDescriptor.UniqueName, cellDescriptor);
                            }
                        }
                    }
                }
            }
            else if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
            {
                foreach (var item in olapReport.CategoricalElements)
                {
                    DimensionElement dimensionElement = item.ElementValue as DimensionElement;

                    if (dimensionElement != null)
                    {
                        if (dimensionElement.Hierarchy != null)
                        {
                            if (dimensionElement.Hierarchy.LevelElements[0].Name == cellDescriptor.UniqueName)
                            {
                                if (!CheckMemberDrillDown(dimensionElement.Hierarchy.LevelElements[0].MemberElements, cellDescriptor.CellValue))
                                {
                                    dimensionElement.Hierarchy.LevelElements[0].MemberElements.Add(new MemberElement { Name = cellDescriptor.CellValue, UniqueName = cellDescriptor.UniqueName + "." + cellDescriptor.CellValue, Level = 0 });
                                }
                                else
                                {
                                    dimensionElement.Hierarchy.LevelElements[0].MemberElements.RemoveAt(GetElementIndex(dimensionElement.Hierarchy.LevelElements[0].MemberElements, cellDescriptor.CellValue));
                                }
                            }
                            else
                            {
                                DrillUpDown(dimensionElement.Hierarchy.LevelElements[0], cellDescriptor.UniqueName, cellDescriptor);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets the index of the element.
        /// </summary>
        /// <param name="memberElementCollection">The member element collection.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <returns></returns>
        private static int GetElementIndex(MemberElementCollection memberElementCollection, string cellValue)
        {
            for (int item = 0; item < memberElementCollection.Count; item++)
            {
                if (memberElementCollection[item].Name == cellValue)
                {
                    return item;
                }
            }
            return 0;
        }

        /// <summary>
        /// Checks the member drill down.
        /// </summary>
        /// <param name="memberElementCollection">The member element collection.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <returns></returns>
        private static bool CheckMemberDrillDown(MemberElementCollection memberElementCollection, string cellValue)
        {
            for (int item = 0; item < memberElementCollection.Count; item++)
            {
                if (memberElementCollection[item].Name == cellValue)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Based on the member passed it will drill down to its child members
        /// </summary>
        /// <param name="model">The Data Manager object</param>
        /// <param name="memberElement">The member element.</param>
        /// <param name="drillDownParenMember">The drill down paren member.</param>
        /// <returns>returns true if drill up and down are successfulll</returns>
        /// <remarks>
        /// Internally it added the childs members to the Element object, so that query genereator picks up
        /// the child member and generates the query
        /// </remarks>
        private static bool DrillUpDown(MemberElement memberElement, string uniqueName, PivotCellDescriptor cellDesc)
        {
            bool isDrillUpDown = false;
            {
                if (memberElement.UniqueName == uniqueName)
                {
                    if (memberElement.ChildMemberElements.Count > 0)
                    {
                        memberElement.ChildMemberElements.Clear();
                    }
                }
                else
                {
                    if (memberElement.ChildMemberElements.Count > 0)
                    {
                        foreach (MemberElement _memberElement in memberElement.ChildMemberElements)
                        {
                            if (DrillUpDown(_memberElement, uniqueName, cellDesc))
                            {
                                isDrillUpDown = true;
                                break;
                            }
                        }
                    }
                }
                if (!isDrillUpDown)
                {
                    if (CheckParentMembers(memberElement, cellDesc.ParentCellValues))
                    {
                        if (memberElement.UniqueName == cellDesc.ParentCellValues[cellDesc.ParentCellValues.Count - 1])
                        {
                            MemberElement childMemberElement = memberElement.ChildMemberElements.FindMemberElementByName(cellDesc.CellValue);
                            if (childMemberElement == null)
                            {
                                childMemberElement = new MemberElement(memberElement);
                                childMemberElement.UniqueName = cellDesc.UniqueName + "." + cellDesc.CellValue;
                                childMemberElement.Name = cellDesc.CellValue;
                                childMemberElement.ShowChildMembers = true;
                                childMemberElement.Level = childMemberElement.ParentMemberElement.Level + 1;
                                memberElement.Add(childMemberElement);
                                isDrillUpDown = true;
                            }
                            else
                            {
                                memberElement.ChildMemberElements.Remove(childMemberElement);
                                isDrillUpDown = true;
                            }
                        }
                    }
                }
            }

            return isDrillUpDown;
        }

        /// <summary>
        /// Checks the parent members.
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private static bool CheckParentMembers(MemberElement memberElement, List<string> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (memberElement != null)
                {
                    if (memberElement.UniqueName != list[i])
                    {
                        return false;
                    }
                    memberElement = memberElement.ParentMemberElement;
                }
            }
            return true;
        }


        /// <summary>
        /// Drills up down.
        /// </summary>
        /// <param name="levelElement">The level element.</param>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <param name="cellDesc">The cell desc.</param>
        private static void DrillUpDown(LevelElement levelElement, string uniqueName, PivotCellDescriptor cellDesc)
        {
            bool isDrillUpDown = false;
            if (levelElement.MemberElements.Count > 0)
            {
                int memberCount = levelElement.MemberElements.Count;
                for (int i = 0; i < memberCount; i++)
                {
                    MemberElement memberElement = levelElement.MemberElements[i];
                    {
                        isDrillUpDown = DrillUpDown(memberElement, uniqueName, cellDesc);
                        if (isDrillUpDown)
                        {
                            break;
                        }
                    }
                }
            }
            if (!isDrillUpDown)
            {
                if (levelElement.UniqueName == uniqueName)
                {
                    MemberElement memberElement = levelElement.MemberElements.FindMemberElementByName(uniqueName);
                    if (memberElement == null)
                    {
                        memberElement = new MemberElement(levelElement);
                        memberElement.UniqueName = cellDesc.UniqueName + "." + cellDesc.CellValue;
                        memberElement.Name = cellDesc.CellValue;
                        memberElement.ShowChildMembers = true;
                        memberElement.Level = memberElement.ParentMemberElement.Level + 1;
                        levelElement.Add(memberElement);
                        isDrillUpDown = true;
                    }
                    else
                    {
                        levelElement.MemberElements.Remove(memberElement);
                        isDrillUpDown = true;
                    }
                }
            }
        }

        /// <summary>
        /// Checks the member drill down.
        /// </summary>
        /// <param name="levelElementCollection">The level element collection.</param>
        /// <param name="value">The value.</param>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <returns></returns>
        private bool CheckMemberDrillDown(LevelElementCollection levelElementCollection, string value, string uniqueName)
        {
            foreach (LevelElement item in levelElementCollection)
            {
                if (item.Name == uniqueName)
                {
                    foreach (MemberElement member in item.MemberElements)
                    {
                        if (member.Name == value)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}
