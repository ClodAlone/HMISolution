//-------------------------------------------------------------------------------------------------
// <copyright file="QueryBuilderEngineHelper.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Linq;

#if !SILVERLIGHT
using System.Diagnostics;
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Reports;
using Syncfusion.Linq;
using Syncfusion.Olap.DataProvider;
using System.Collections.Generic;

namespace Syncfusion.Olap.MDXQueryBuilder
#else
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Manager;
using System.Collections.Generic;

namespace Syncfusion.OlapSilverlight.MDXQueryBuilder
#endif
{
    /// <summary>
    /// This EngineHelper class is used for helping the QueryBuilderEngine to populate items
    /// from other assemblies.  
    /// </summary>
    public class QueryBuilderEngineHelper
    {
        internal static Providers ProviderName { get; set; }

        #region Public Methods

        public static void SetProviderName(Providers providerName)
        {
            QueryBuilderEngineHelper.ProviderName = providerName;
        }

        /// <summary>
        /// Update the MDXQuerySpecification object with new elements based on the 
        /// MetaTreeNodeCollection passed
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <param name="mtNodeCollection">The mt node collection.</param>
        /// <param name="axis">The axis position</param>
        public static void FillMDXQuerySpecification(MDXQuerySpecification mdxQuery, MetaTreeNodeCollection mtNodeCollection, AxisPosition axis)
        {
            mdxQuery.Select.Items.AddRange(GetElementItemsFromMetaTree(mtNodeCollection, axis));
        }


        

        /// <summary>
        /// Builds the element items from meta tree.
        /// </summary>
        /// <param name="mtNodeCollection">The mt node collection.</param>
        /// <param name="axis">The axis position</param>
        /// <returns>returns Items from metatree</returns>
        public static Items GetElementItemsFromMetaTree(MetaTreeNodeCollection mtNodeCollection, AxisPosition axis)
        {
            Items items = new Items();
            foreach (MetaTreeNode metaTreeNode in mtNodeCollection)
            {
                if (metaTreeNode.Properties.Count > 0)
                {
                    items.Add(new Item
                    {
                        Axis = axis,
                        ElementValue = GetElementValue(metaTreeNode, false),
                        ExcludedElementValue = GetExcludedDimensionElement(metaTreeNode)
                    });
                }
            }

            return items;
        }

        /// <summary>
        /// Gets the element item from meta tree.
        /// </summary>
        /// <param name="metaTreeNode">The meta tree node.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>An item node.</returns>
        public static Item GetElementItemFromMetaTree(MetaTreeNode metaTreeNode, AxisPosition axis)
        {
            Item item = null;
            {
                if (metaTreeNode.Properties.Count > 0)
                {
                    item = (new Item
                    {
                        Axis = axis,
                        ElementValue = GetElementValue(metaTreeNode, false)
                    });
                }
            }

            return item;
        }

        public static List<string> GetUniqueNames<T>(T item)
        {
            List<string> uniqnameCollec = new List<string>();
            if (item is LevelElementCollection)
            {
                LevelElementCollection levelCollec = item as LevelElementCollection;
                if (levelCollec != null && levelCollec.Count > 0)
                {
                    foreach (LevelElement level in levelCollec)
                    {
                        uniqnameCollec.Add(level.UniqueName);
                    }
                }
            }
            else if(item is MemberElementCollection)
            {
                MemberElementCollection elementCollec = item as MemberElementCollection;
                if (elementCollec != null && elementCollec.Count > 0)
                {
                    foreach (MemberElement elemnt in elementCollec)
                    {
                        uniqnameCollec.Add(elemnt.UniqueName);
                    }
                }

            }
            return uniqnameCollec;
        }

         public static MemberElementCollection GetReportMemberElements(MemberElement memElement, MemberElementCollection reportElementCollection)
        {
           
            if (memElement.ChildMemberElements.Count > 0)
            {
                foreach (MemberElement membElement in memElement.ChildMemberElements)
                {
                    reportElementCollection.Add(membElement);
                    GetReportMemberElements(membElement, reportElementCollection);
                }
            }
            return reportElementCollection;
        }

         private static  void GetReportMembers(DimensionElement inputCollection, MemberElementCollection resultCollection)
         {
             LevelElementCollection levelElementCollection =inputCollection.Hierarchy.LevelElements;

             foreach (LevelElement levElement in levelElementCollection)
             {
                 MemberElementCollection memElements = levElement.MemberElements;
                 foreach (MemberElement memElement in memElements)
                 {
                     resultCollection.Add(memElement);
                     GetReportMemberElements(memElement, resultCollection);
                 }
             }
         }

        /// <summary>
        /// Updates the items from meta tree.
        /// </summary>
        /// <param name="mtNodeCollection">The Meta tree node collection.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="items">The items.</param>
        public static void UpdateItemsFromMetaTree(MetaTreeNodeCollection mtNodeCollection, AxisPosition axis, Items items)
        {
            

            foreach (MetaTreeNode metaTreeNode in mtNodeCollection)
            {
                if (metaTreeNode.Properties.Count > 0)
                {
                    Item item = FindItemsFromMetaTreeNode(metaTreeNode, items);
                  
                    if (item != null)
                    {
                        item.Axis = axis;
#if SILVERLIGHT
                        MemberElementCollection processedExcludedElements = new MemberElementCollection();

                        MemberElementCollection processedElements = new MemberElementCollection();

                        MemberElementCollection reprtExcludedElement = new MemberElementCollection();

                        MemberElementCollection excludedElement = new MemberElementCollection();

                        MemberElementCollection elements = new MemberElementCollection();

                        MemberElementCollection reportelements = new MemberElementCollection();

                        if (item.ElementValue is DimensionElement)
                        {
                            if (ProviderName == Providers.SSAS)
                            {
                                DimensionElement excludeElement = GetExcludedDimensionElement(metaTreeNode);

                                if (excludedElement != null && excludedElement.Count > 0)
                                {
                                    GetReportMembers(excludeElement, excludedElement);
                                }



                                DimensionElement reportExcludedElement = item.ExcludedElementValue as DimensionElement;

                                if (reportExcludedElement != null)
                                {
                                    GetReportMembers(reportExcludedElement, reprtExcludedElement);
                                }

                                DimensionElement reportElements = item.ElementValue as DimensionElement;

                                if (reportElements != null)
                                {
                                    GetReportMembers(reportElements, reportelements);
                                }

                                Element elementValue = GetElementValue(metaTreeNode, true);

                                DimensionElement elementfrmtre = elementValue as DimensionElement;

                                if (elementfrmtre != null)
                                {
                                    GetReportMembers(elementfrmtre, elements);
                                }

                                List<string> reportExcluElemntUniqNames = GetUniqueNames<MemberElementCollection>(reprtExcludedElement);

                                List<string> excludElemUniqNames = GetUniqueNames<MemberElementCollection>(excludedElement);

                                List<string> elemtUniqNames = GetUniqueNames<MemberElementCollection>(elements);

                                List<string> reportElemtUniqNames = GetUniqueNames<MemberElementCollection>(reportelements);

                                foreach (MemberElement reprtExcElet in reprtExcludedElement)
                                {
                                    if (excludElemUniqNames != null && elemtUniqNames != null)
                                    {
                                        if ((!excludElemUniqNames.Contains(reprtExcElet.UniqueName)) && (!elemtUniqNames.Contains(reprtExcElet.UniqueName)))
                                        {
                                            processedElements.Add(reprtExcElet);
                                        }
                                    }
                                }


                                if (excludeElement != null)
                                {
                                    for (int j = 0; j < excludeElement.Hierarchy.LevelElements.Count; j++)
                                    {
                                        

                                        foreach (MemberElement proExElet in processedElements)
                                        {
                                            List<string> levelUniqNames = GetUniqueNames<LevelElementCollection>(excludeElement.Hierarchy.LevelElements);

                                            string[] leveluniqnam = excludeElement.Hierarchy.LevelElements[j].UniqueName.Split('.');

                                            string[] proExEletuniqnam = proExElet.UniqueName.Split('.');

                                            string proExEletLevUniqName = proExEletuniqnam[0] + "." + proExEletuniqnam[1] + "." + proExEletuniqnam[2];

                                            if (leveluniqnam[2] == proExEletuniqnam[2])
                                            {
                                                if (excludeElement.Hierarchy.LevelElements[j].MemberElements.Count == 0)
                                                    excludeElement.Hierarchy.LevelElements[j].MemberElements.Add(proExElet);
                                                else
                                                {
                                                    List<string> membuniqnam = new List<string>();
                                                    membuniqnam = GetUniqueNames<MemberElementCollection>(excludeElement.Hierarchy.LevelElements[j].MemberElements);
                                                    if (!membuniqnam.Contains(proExElet.UniqueName))
                                                        excludeElement.Hierarchy.LevelElements[j].MemberElements.Add(proExElet);
                                                }
                                            }
                                            else if (!levelUniqNames.Contains(proExEletLevUniqName))
                                            {
                                                if (reportExcludedElement != null)
                                                {
                                                    for (int k = 0; k < reportExcludedElement.Hierarchy.LevelElements.Count; k++)
                                                    {
                                                        string[] replevuniqnam = reportExcludedElement.Hierarchy.LevelElements[k].UniqueName.Split('.');
                                                        if (replevuniqnam[2] == proExEletuniqnam[2])
                                                        {
                                                            LevelElement levEle = reportExcludedElement.Hierarchy.LevelElements[k];
                                                            levEle.MemberElements.Clear();
                                                            levEle.MemberElements.Add(proExElet);
                                                            excludeElement.Hierarchy.LevelElements.Add(levEle);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                foreach (MemberElement reprtElemt in reportelements)
                                {
                                    if (elemtUniqNames != null)
                                    {
                                        if (elemtUniqNames.Contains(reprtElemt.UniqueName))
                                        {
                                            foreach (MemberElement metrElemt in elements)
                                            {
                                                if (reprtElemt.ChildMemberElements.Count > 0 && metrElemt.ChildMemberElements.Count == 0)
                                                {
                                                    string[] repeleuniqnam = reprtElemt.UniqueName.Split('.');
                                                    string[] metreleuniqnam = metrElemt.UniqueName.Split('.');
                                                    if (repeleuniqnam[2] == metreleuniqnam[2])
                                                    {
                                                        for (int i = 0; i < elementfrmtre.Hierarchy.LevelElements[0].MemberElements.Count; i++)
                                                        {
                                                            if (elementfrmtre.Hierarchy.LevelElements[0].MemberElements[i].UniqueName == reprtElemt.UniqueName)
                                                            {
                                                                elementfrmtre.Hierarchy.LevelElements[0].MemberElements.RemoveAt(i);
                                                                elementfrmtre.Hierarchy.LevelElements[0].MemberElements.Add(reprtElemt);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                item.ExcludedElementValue = excludeElement; //GetExcludedDimensionElement(metaTreeNode); //
                                //if (axis == AxisPosition.Slicer)
                                item.ElementValue = elementValue; //GetElementValue(metaTreeNode, true); //
                                if (excludeElement != null)
                                    RemoveExcludedElements(item.ElementValue as DimensionElement, excludeElement);
                            }
                            else
                            {
                                DimensionElement excldedElement = GetExcludedDimensionElement(metaTreeNode);
                                item.ExcludedElementValue = excldedElement;
                                //if (axis == AxisPosition.Slicer)
                                    item.ElementValue = GetElementValue(metaTreeNode, true);
                                if (excldedElement != null)
                                    RemoveExcludedElements(item.ElementValue as DimensionElement, excldedElement);
                            }
                        }


#else
                        
                        if (item.ElementValue is DimensionElement)
                        {
                            DimensionElement excludeElement = GetExcludedDimensionElement(metaTreeNode);
                            item.ExcludedElementValue = excludeElement;
                            //if (axis == AxisPosition.Slicer)
                                item.ElementValue = GetElementValue(metaTreeNode, true);
                            if (excludeElement != null)
                                RemoveExcludedElements(item.ElementValue as DimensionElement, excludeElement);
                        }
#endif
                        else if (item.ElementValue is MeasureElements)
                        {
                            MeasureElements measureElements = new MeasureElements();
                            SynchronizeMeasureElements(measureElements, metaTreeNode);
                            item.ElementValue = measureElements;
                            //// if no element's selected then remove the item form the collection
                            if (measureElements.Elements.Count == 0)
                            {
                                items.Remove(item);
                            }
                        }
                        else if (item.ElementValue is KpiElements)
                        {
                            KpiElements kpiElements = new KpiElements();
                            SynchronizeKpiElements(kpiElements, metaTreeNode);
                            item.ElementValue = kpiElements;
                            //// if no element's selected then remove the item from the collection 
                            if (kpiElements.Elements.Count == 0)
                            {
                                items.Remove(item);
                            }
                        }
                        else if (item.ElementValue is NamedSetElement)
                        {
                            NamedSetElement namedSetElement = new NamedSetElement();
                            SynchronizeNamedSetElement(namedSetElement, metaTreeNode);
                            item.ElementValue = namedSetElement;
                        }
                        else if (item.ElementValue is CalculatedMember)
                        {
                            CalculatedMember calcMember = new CalculatedMember();
                            SynchronizeCalculatedMembers(calcMember, metaTreeNode);
                            item.ElementValue = calcMember;
                        }
                        else if (item.ElementValue is VirtualKpiElement)
                        {
                            VirtualKpiElement virtualKpiElement = new VirtualKpiElement();
                            SynchronizeVirtualKpiElement(virtualKpiElement, metaTreeNode);
                            item.ElementValue = virtualKpiElement;
                            if (!(virtualKpiElement.ShowVirtualKPIGoal) && !(virtualKpiElement.ShowVirtualKPIStatus) && !(virtualKpiElement.ShowVirtualKPITrend) && !(virtualKpiElement.ShowVirtualKPIValue))
                            {
                                items.Remove(item);
                            }
                        }
                    }
                    else
                    {
                        Item newItem = GetElementItemFromMetaTree(metaTreeNode, axis);
                        newItem.ExcludedElementValue = GetExcludedDimensionElement(metaTreeNode);
                        if (newItem.ElementValue is VirtualKpiElement)
                        {
                            if ((newItem.ElementValue as VirtualKpiElement).ShowVirtualKPIGoal || (newItem.ElementValue as VirtualKpiElement).ShowVirtualKPIStatus || (newItem.ElementValue as VirtualKpiElement).ShowVirtualKPITrend || (newItem.ElementValue as VirtualKpiElement).ShowVirtualKPIValue)
                            {
                                items.Add(newItem);
                            }
                        }
                        else
                        {
                            items.Add(newItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Based on the member passed it will drill down to its child members
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        /// <param name="drillDownParentMember">The drill down parent member.</param>
        /// <returns>returns true if drill up and down are successfully</returns>
        /// <remarks>
        /// Internally it added the children members to the Element object, so that query generator picks up
        /// the child member and generates the query
        /// </remarks>
        public static bool DrillUpDown(MemberElement memberElement, Member drillDownParentMember)
        {
            bool isDrillUpDown = false;
            {
                if (memberElement.UniqueName == drillDownParentMember.UniqueName || memberElement.UniqueName == drillDownParentMember.CustomUniqueName)
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
                            if (DrillUpDown(_memberElement, drillDownParentMember))
                            {
                                isDrillUpDown = true;
                                break;
                            }
                        }
                    }
                }

                if (!isDrillUpDown)
                {
                    string drillDownParentMemberUniqueName = string.Empty;
                    if (drillDownParentMember.ParentUniqueName == string.Empty && drillDownParentMember.ParentCustomUniqueName == string.Empty)
                    {
                        int index1 = drillDownParentMember.UniqueName.LastIndexOf('.');
                        drillDownParentMemberUniqueName = drillDownParentMember.UniqueName.Remove(index1, drillDownParentMember.UniqueName.Length - index1);
                    }
                    if (memberElement.UniqueName == drillDownParentMember.ParentUniqueName || memberElement.UniqueName == drillDownParentMember.ParentCustomUniqueName || memberElement.UniqueName == drillDownParentMemberUniqueName)
                    {
                        MemberElement childMemberElement = memberElement.ChildMemberElements.FindMemberElementByUniqueName(drillDownParentMember.UniqueName);
                        if (childMemberElement == null)
                        {
                            childMemberElement = new MemberElement(memberElement);
                            childMemberElement.UniqueName = drillDownParentMember.UniqueName;
                            childMemberElement.Name = drillDownParentMember.Caption;
                            childMemberElement.ShowChildMembers = true;
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

            return isDrillUpDown;
        }

        /// <summary>
        /// Drills up down hierarchyElement based on the drilldownParentMember.
        /// </summary>
        /// <param name="hierarchyElement">The hierarchy element.</param>
        /// <param name="drillDownParentMember">The drill down parent member.</param>
        /// <returns>true if children are appended else returns false</returns>
        public static bool DrillUpDown(HierarchyElement hierarchyElement, Member drillDownParentMember)
        {
            bool isDrillUpDown = false;
            LevelElement levelElement;
            if (hierarchyElement.LevelElements.Count > 0)
            {
                levelElement = hierarchyElement.LevelElements[0];
            }
            else
            {
                throw new Exception("LevelElement should be specified");
            }

            if (levelElement.MemberElements.Count > 0)
            {
                int memberCount = levelElement.MemberElements.Count;
                for (int i = 0; i < memberCount; i++)
                {
                    MemberElement memberElement = levelElement.MemberElements[i];
                    {
                        isDrillUpDown = DrillUpDown(memberElement, drillDownParentMember);
                        if (isDrillUpDown)
                        {
                            return isDrillUpDown;
                        }
                    }
                }
            }

            if (!isDrillUpDown)
            {
                //if (levelElement.UniqueName == drillDownParentMember.LevelUniqueName)
                {
                    MemberElement memberElement = levelElement.MemberElements.FindMemberElementByUniqueName(drillDownParentMember.UniqueName);
                    if (memberElement == null)
                    {
                        memberElement = new MemberElement(levelElement);
                        memberElement.UniqueName = drillDownParentMember.UniqueName;
                        memberElement.Name = drillDownParentMember.Caption;
                        memberElement.ShowChildMembers = true;
                        hierarchyElement.LevelElements[0].Add(memberElement);
                        isDrillUpDown = true;
                    }
                    else
                    {
                        if (!levelElement.IncludeAvailableMembers)
                        {
                            levelElement.MemberElements.Remove(memberElement);
                        }
                        else
                        {
                            memberElement.ShowChildMembers = !memberElement.ShowChildMembers;
                        }

                        isDrillUpDown = true;
                    }
                }
            }

            return isDrillUpDown;
        }

        /// <summary>
        /// Updates the drill down items based on the Categorical or Series Items passed internally.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="member">The member.</param>
        /// <returns>Returns true if drilldown happens.  Always returns true.</returns>
        public static bool UpdateDrillDownItems(Items items, Member member)
        {
#if !SILVERLIGHT
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
#endif
            bool isDataModified = false;
            string[] memberNames = member.UniqueName.Split('.');
            foreach (Item item in items)
            {
                DimensionElement dimensionElement = item.ElementValue as DimensionElement;
                if (dimensionElement != null)
                {
                    if (dimensionElement.UniqueName.ToUpper() == memberNames[0].ToUpper() && member.LevelUniqueName.ToUpper().Contains("[" + dimensionElement.Hierarchy.Name.ToUpper() + "]"))
                    {
                        if (DrillUpDown(dimensionElement.Hierarchy, member))
                        {
                            isDataModified = true;
                            break;
                        }
                    }
                }
            }
#if !SILVERLIGHT
#if DEBUG
            sw.Stop();
            Console.WriteLine("Time taken form QueryBuilderEngineHelper.UpdateDrillDownItems -- Query Generation {0}", sw.Elapsed);
            sw.Reset();
#endif
#endif

            return isDataModified;
        }

        /// <summary>
        /// Generates the element based on selected and unselected nodes
        /// </summary>
        /// <param name="metaTreeNode">MetaTreeNodes</param>
        /// <param name="includeSelectedElements">flag indicated weather to include the selected nodes in elements collection or not</param>
        /// <returns></returns>
        public static Element GetElementValue(MetaTreeNode metaTreeNode, bool includeSelectedElements)
        {
            Element element = null;
            if (metaTreeNode.Properties.Count > 0)
            {
                if (metaTreeNode.Properties[0].Value is MeasureCollection)
                {
                    MeasureElements elements = new MeasureElements();
                    SynchronizeMeasureElements(elements, metaTreeNode);
                    return elements;
                }
                else if (metaTreeNode.Properties[0].Value is KpiCollection)
                {
                    KpiElements kpiElement = new KpiElements();
                    SynchronizeKpiElements(kpiElement, metaTreeNode);
                    return kpiElement;
                }
                else if (metaTreeNode.Properties[0].Value is NamedSet)
                {
                    NamedSetElement namedSetElement = new NamedSetElement();
                    SynchronizeNamedSetElement(namedSetElement, metaTreeNode);
                    return namedSetElement;
                }
                else if (metaTreeNode.Properties[0].Value is CalculatedMember)
                {
                    CalculatedMember calcMemberElement = new CalculatedMember();
                    SynchronizeCalculatedMembers(calcMemberElement, metaTreeNode);
                    return calcMemberElement;
                }
                else if (metaTreeNode.Properties[0].Value is VirtualKpiElement)
                {
                    VirtualKpiElement virtualKpiElement = new VirtualKpiElement();
                    SynchronizeVirtualKpiElement(virtualKpiElement, metaTreeNode);
                    return virtualKpiElement;
                }
                else
                {
                    //// Synchronizing Dimension and level elements
                    if (metaTreeNode.Properties[0].Value is Dimension)
                    {
                        element = GetDimensionElement(metaTreeNode, includeSelectedElements);
                    }
                    else if (metaTreeNode.Properties[0].Value is Level)
                    {
                        element = GetLevelElement(metaTreeNode);
                    }
                }
            }

            return element;
        }
        #endregion

        #region Internal Methods
#if !SILVERLIGHT
        /// <summary>
        /// Gets the member element from the Member object.
        /// </summary>
        /// <param name="memberObj">The member obj.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <returns>The MemberElement from the member Object</returns>
        internal static MemberElement GetMemberElement(Member memberObj, bool isVisible)
        {
            MemberElement memberElelment = new MemberElement();
            memberElelment.Name = memberObj.Name;
            memberElelment.UniqueName = memberObj.UniqueName;
            memberElelment.Visible = isVisible;
            memberElelment.Properties.Add(PropertyConstants.Member, memberObj);
            return memberElelment;
        }

        /// <summary>
        /// Gets the level element from Level Object and adds it to the Hierarchy Element.
        /// </summary>
        /// <param name="parentHierarchyElement">The parent hierarchy element.</param>
        /// <param name="levelObj">The level obj.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <returns>Level Element from the level Object</returns>
        internal static LevelElement GetLevelElement(HierarchyElement parentHierarchyElement, Level levelObj, bool isVisible)
        {
            LevelElement levelElement = new LevelElement(parentHierarchyElement);
            levelElement.Name = levelObj.Name;
            levelElement.Visible = isVisible;
            levelElement.Properties.Add(PropertyConstants.Level, levelObj);
            return levelElement;
        }
#endif
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds the member to level if Level not found then creates a new level and inserts
        /// the Member in it.
        /// </summary>
        /// <param name="excludedDimensionElement">The excluded dimension element.</param>
        /// <param name="excludedMember">The excluded member.</param>
        /// <param name="hierarchyObj">The hierarchy object</param>
        /// <param name="levelObj">The Level object</param>
        /// <returns>
        /// The Excluded DimensionElement along with excluded members
        /// </returns>
        private static DimensionElement AddMemberToLevel(ref DimensionElement excludedDimensionElement, Member excludedMember, Hierarchy hierarchyObj, Level levelObj)
        {
            if (excludedDimensionElement == null)
            {
                excludedDimensionElement = new DimensionElement();
                excludedDimensionElement.Name = hierarchyObj.ParentDimension.Name;
                excludedDimensionElement.AddLevel(hierarchyObj.Name, levelObj.Name);
                excludedDimensionElement.Hierarchy.IsAttributeHierarchy = hierarchyObj.IsAttributeHierarchy;
            }

            LevelElement levelElement = null;
            foreach (LevelElement lvlElements in excludedDimensionElement.Hierarchy.LevelElements)
            {
                if (lvlElements.UniqueName == excludedMember.LevelUniqueName || lvlElements.UniqueName == hierarchyObj.UniqueName + "." + excludedMember.LevelUniqueName)
                {
                    levelElement = lvlElements;
                }
            }

            if (levelElement != null)
            {
                levelElement.IncludeAvailableMembers = true;
                if (string.IsNullOrEmpty(excludedMember.UniqueName))
                {
                    levelElement.Add(excludedMember.Caption, excludedMember.LevelDepth);
                }
                else
                {
                    levelElement.Add(excludedMember.Caption,excludedMember.ParentCaption, excludedMember.UniqueName, excludedMember.LevelDepth);
                }
            }
            else
            {
                string[] s = excludedMember.LevelUniqueName.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                string levelName;
                if(s.Length == 3)
                    levelName = s[2];
                else
                    levelName = s[4];
                excludedDimensionElement.Hierarchy.Add(levelName);
                if (string.IsNullOrEmpty(excludedMember.UniqueName))
                {
                    excludedDimensionElement.Hierarchy.LevelElements[levelName].Add(excludedMember.Caption, excludedMember.LevelDepth);
                }
                else
                {
                    excludedDimensionElement.Hierarchy.LevelElements[levelName].Add(excludedMember.Caption,excludedMember.ParentCaption, excludedMember.UniqueName, excludedMember.LevelDepth);
                }
                excludedDimensionElement.Hierarchy.LevelElements[levelName].IncludeAvailableMembers = true;
            }

            return excludedDimensionElement;
        }

        public static Item FindItemsFromMetaTreeNode(MetaTreeNode metaTreeNode, Items items)
        {
            foreach (Item item in items)
            {
#if !SILVERLIGHT
                if ((metaTreeNode.NodeType == MetaTreeNodeType.CalculatedMember && item.ElementValue is CalculatedMember && metaTreeNode.UniqueName.ToUpper() == (item.ElementValue as CalculatedMember).UniqueName.ToUpper())
                    || (item.ElementValue is MeasureElements && metaTreeNode.Name == PropertyConstants.MeasrueNodeName)
                    || (item.ElementValue is NamedSetElement && item.ElementValue.Name.ToUpper() == metaTreeNode.Name.ToUpper())
                    || (item.ElementValue is KpiElements && metaTreeNode.Name == PropertyConstants.KPI)
                    || ((item.ElementValue is VirtualKpiElement) && metaTreeNode.NodeType == MetaTreeNodeType.VirtualKPIMember && (item.ElementValue as VirtualKpiElement).UniqueName.ToUpper() == metaTreeNode.UniqueName.ToUpper()))
#else
                if ((metaTreeNode.NodeType == MetaTreeNodeType.CalculatedMember && item.ElementValue is CalculatedMember && metaTreeNode.UniqueName.ToUpper() == (item.ElementValue as CalculatedMember).UniqueName.ToUpper())
                    || (item.ElementValue is MeasureElements && metaTreeNode.Name == PropertyConstants.MeasrueNodeName)
                    || (item.ElementValue is NamedSetElement && item.ElementValue.Name.ToUpper() == metaTreeNode.Name.ToUpper())
                    || (item.ElementValue is KpiElements && metaTreeNode.Name == PropertyConstants.KPI)
                    || (item.ElementValue is VirtualKpiElement) && metaTreeNode.NodeType == MetaTreeNodeType.VirtualKPIMember && metaTreeNode.UniqueName.ToUpper()==(item.ElementValue as VirtualKpiElement).UniqueName.ToUpper())
#endif
                {
                    return item;
                }
                else if (item.ElementValue !=null && item.ElementValue.Name != null && (item.ElementValue.Name.ToUpper() == metaTreeNode.Name.ToUpper() || item.ElementValue.Name.ToUpper() == metaTreeNode.Caption.ToUpper()))
                {
                    {
                        if (((Property)((PropertyCollection)metaTreeNode.Properties)[2]).Name == "HIERARCHY")
                        {
                            if (((DimensionElement)item.ElementValue).HierarchyName.ToUpper() == ((Hierarchy)((Property)((PropertyCollection)metaTreeNode.Properties)[2]).Value).Name.ToUpper())
                            {
                                if (((LevelElementCollection)((HierarchyElement)((DimensionElement)item.ElementValue).Hierarchy).LevelElements)[0].Name.ToUpper() == ((Level)((Property)((PropertyCollection)metaTreeNode.Properties)[1]).Value).Name.ToUpper())
                                {
                                    return item;
                                }
                            }
                        }
                        else
                        {
                            if (((DimensionElement)item.ElementValue).HierarchyName.ToUpper() == ((Hierarchy)((Property)((PropertyCollection)metaTreeNode.Properties)[1]).Value).Name.ToUpper())
                            {
                                if (((LevelElementCollection)((HierarchyElement)((DimensionElement)item.ElementValue).Hierarchy).LevelElements)[0].Name.ToUpper() == ((Level)((Property)((PropertyCollection)metaTreeNode.Properties)[2]).Value).Name.ToUpper()
                                    || ((LevelElementCollection)((HierarchyElement)((DimensionElement)item.ElementValue).Hierarchy).LevelElements)[0].UniqueName.ToUpper() == ("[" + metaTreeNode.Name + "].[" + ((Hierarchy)((Property)((PropertyCollection)metaTreeNode.Properties)[1]).Value).Name + "].[]").ToUpper())
                                {
                                    return item;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        static DimensionElement GetExcludedDimensionElement(MetaTreeNode metaTreeNode)
        {
            if (metaTreeNode.Properties[0].Value is Dimension)
            {
                Dimension dimensionObj = metaTreeNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                Hierarchy hierarchyObj = metaTreeNode.Properties.FindByName(PropertyConstants.Hierarchy).Value as Hierarchy;
                Level levelObj = metaTreeNode.Properties.FindByName(PropertyConstants.Level).Value as Level;
                if (dimensionObj != null)
                {
                    DimensionElement dimensionElement = null;
                    ////dimensionElement.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                    if (metaTreeNode.ChildNodes.Count > 0)
                    {
                        foreach (MetaTreeNode hierarchymetaTreeNode in metaTreeNode.ChildNodes)
                        {
                            if (hierarchymetaTreeNode.NodeType == MetaTreeNodeType.Member)
                            {
                                Member memberObj = hierarchymetaTreeNode.Properties.FindByName(PropertyConstants.Member).Value as Member;
                                if (hierarchymetaTreeNode.NodeCheckedType == MetaTreeNodeCheckedType.NoneSelected)
                                {
                                    dimensionElement = UpdateMemberElement(ref dimensionElement, hierarchymetaTreeNode, hierarchyObj, levelObj);
                                }
                                else if (hierarchymetaTreeNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                                {
                                    if (hierarchymetaTreeNode.ChildNodes.Count > 0)
                                    {
                                        foreach (MetaTreeNode memberMetaTreeNode in hierarchymetaTreeNode.ChildNodes)
                                        {
                                            if (memberMetaTreeNode.Caption != "(Blank)")
                                                dimensionElement = UpdateMemberElement(ref dimensionElement, memberMetaTreeNode, hierarchyObj, levelObj);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return dimensionElement;
                }
            }

            return null;
        }

        static void RemoveExcludedElements(DimensionElement parentDimensionElement, DimensionElement excludedDimensionElement)
        {
            foreach (LevelElement levelElement in excludedDimensionElement.Hierarchy.LevelElements)
            {
                foreach (MemberElement excludeMemberElement in levelElement.MemberElements)
                {
                    MemberElement element = null;
                    if (parentDimensionElement.Hierarchy != null && parentDimensionElement.Hierarchy.LevelElements.Count > 0
                        && parentDimensionElement.Hierarchy.LevelElements[0].MemberElements != null)
                    {
                        foreach (MemberElement memberElement in parentDimensionElement.Hierarchy.LevelElements[0].MemberElements)
                        {
                            if (memberElement.Name == excludeMemberElement.Name && memberElement.UniqueName == excludeMemberElement.UniqueName)
                                element = memberElement;
                        }
                        if (element != null)
                        {
                            parentDimensionElement.Hierarchy.LevelElements[0].MemberElements.Remove(element);
                        }

                    }
                    if (element == null)
                    {
                        Action<MemberElement> looping = null;
                        bool elementRemoved = false;
                        looping = (m) =>
                            {
                                if (element != null)
                                    return;
                                if (m.UniqueName == excludeMemberElement.UniqueName)
                                {
                                    element = m;
                                    return;
                                }

                                if (element == null && m.ChildMemberElements != null)
                                    m.ChildMemberElements.ToList<MemberElement>().ForEach(i => looping(i));
                                if (element != null && !elementRemoved)
                                {
                                    m.ChildMemberElements.Remove(element);
                                    elementRemoved = true;
                                }
                            };

                        if (parentDimensionElement.Hierarchy.LevelElements.Count > 0 && parentDimensionElement.Hierarchy.LevelElements[0].MemberElements != null)
                            parentDimensionElement.Hierarchy.LevelElements[0].MemberElements.ToList<MemberElement>().ForEach(i => looping(i));
                    }
                }
            }
        }


        /// <summary>
        /// Generates the dimension element based on the MetaTreeNode passed
        /// </summary>
        /// <param name="metaTreeNode">MetaTreeNode</param>
        /// <param name="includeSelectedElements">Allow include the selected members</param>
        /// <returns>DimensionElement</returns>
        static DimensionElement GetDimensionElement(MetaTreeNode metaTreeNode, bool includeSelectedElements)
        {
            //// Getting dimension from MetaTreeNode
            Dimension dimensionObj = metaTreeNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
            //// Getting hierarchy from MetaTreeNode
            Hierarchy hierarchyObj = metaTreeNode.Properties.FindByName(PropertyConstants.Hierarchy).Value as Hierarchy;
            //// Getting level from MetaTreeNode
            Level levelObj = metaTreeNode.Properties.FindByName(PropertyConstants.Level).Value as Level;
            if (dimensionObj != null && hierarchyObj != null && levelObj != null)
            {
                DimensionElement dimensionElement = new DimensionElement();
                dimensionElement.Name = dimensionObj.Name;
                dimensionElement.AddLevel(hierarchyObj.Name, levelObj.Name);
                dimensionElement.Hierarchy.IsAttributeHierarchy = hierarchyObj.IsAttributeHierarchy;
                dimensionElement.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                if (includeSelectedElements)
                {
                    MemberElementCollection members = new MemberElementCollection();
                    if (metaTreeNode.ChildNodes.Count > 0)
                    {
                        foreach (MetaTreeNode memberNode in metaTreeNode.ChildNodes)
                        {
                            if (memberNode.NodeType == MetaTreeNodeType.Member)
                            {
                                if (memberNode.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked)
                                {
                                    Member memberObj = memberNode.Properties.FindByName(PropertyConstants.Member).Value as Member;
                                    MemberElement memberElement = new MemberElement();
                                    memberElement.Name = memberObj.Name;
                                    memberElement.UniqueName = memberObj.UniqueName;
                                    members.Add(memberElement);
                                }
                                else if (memberNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                                {
                                    if (memberNode.ChildNodes.Count > 0)
                                    {
                                        MemberElement childMemberElement = GetMemberElement(memberNode);
                                        members.Add(childMemberElement);
                                    }
                                }
                            }
                            else
                            {
                                UpdateHierarchyElement(dimensionElement, memberNode);
                            }
                        }
                    }
                    if (members.Count > 0)
                    {
                        dimensionElement.Hierarchy.LevelElements[levelObj.Name].MemberElements = members;
                    }
                }

                ////Hierarchy hierarchyObj = dimensionObj.Hierarchies.FindByName(hierarchyName);
                //if (hierarchyObj != null)
                //{
                //    ////Level levelObj = hierarchyObj.Levels.FindByName(levelName);
                //    if (levelObj != null)
                //    {
                //        if (hierarchyObj.Name != string.Empty && levelObj.Name != string.Empty)
                //        {
                //            dimensionElement.AddLevel(hierarchyObj.Name, levelObj.Name);
                //            if (memberCount == levelObj.Members.Count)
                //            {
                //                dimensionElement.Hierarchy.LevelElements[levelName].IncludeAvailableMembers = false;
                //            }
                //            else
                //            {
                //                dimensionElement.Hierarchy.LevelElements[levelName].MemberElements = members;
                //                dimensionElement.Hierarchy.LevelElements[levelName].IncludeAvailableMembers = true;
                //            }
                //        }
                //    }
                //}

                return dimensionElement;
            }

            return null;
        }

        /// <summary>
        /// Gets the level element from Metatreenode.
        /// </summary>
        /// <param name="metaTreeNode">The meta tree node.</param>
        /// <returns>The Level Element</returns>
        static Element GetLevelElement(MetaTreeNode metaTreeNode)
        {
            Level levelObj = metaTreeNode.Properties.FindByName(PropertyConstants.Level).Value as Level;
            if (levelObj != null)
            {
                DimensionElement dimensionElement = new DimensionElement();
                Dimension dimensionObj = metaTreeNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                dimensionElement.Name = dimensionObj.Name;
                Hierarchy hierarchyObj = metaTreeNode.Properties.FindByName(PropertyConstants.Hierarchy).Value as Hierarchy;
                HierarchyElement hierarchyElement = new HierarchyElement(dimensionElement);
                dimensionElement.SetHierarchy(hierarchyObj.Name);
                hierarchyElement.Name = hierarchyObj.Name;
                LevelElement levelElement = new LevelElement(hierarchyElement);
                dimensionElement.AddLevel(hierarchyObj.Name, levelObj.Name);
                dimensionElement.Hierarchy.IsAttributeHierarchy = hierarchyObj.IsAttributeHierarchy;
                if (metaTreeNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                {
                    dimensionElement.Hierarchy.LevelElements[levelObj.Name].IncludeAvailableMembers = true;
                }
                else
                {
                    dimensionElement.Hierarchy.LevelElements[levelObj.Name].IncludeAvailableMembers = false;
                }

                levelElement.Name = levelObj.Name;
                levelElement.Properties.Add(new Property(PropertyConstants.Level, levelObj));
                if (metaTreeNode.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode membermetaTreeNode in metaTreeNode.ChildNodes)
                    {
                        UpdateMemberElement(levelElement, membermetaTreeNode);
                    }
                }

                dimensionElement.Hierarchy.LevelElements[levelObj.Name].MemberElements = levelElement.MemberElements;
                dimensionElement.Hierarchy.LevelElements[levelObj.Name].DimensionName = dimensionObj.UniqueName;
                return dimensionElement;
            }

            return null;
        }

        /// <summary>
        /// Gets the member element from Metatreenode.
        /// </summary>
        /// <param name="metaTreeNode">The meta tree node.</param>
        /// <returns>The Member Element</returns>
        static MemberElement GetMemberElement(MetaTreeNode metaTreeNode)
        {
            if (!(metaTreeNode.Properties.Count > 0))
            {
                return null;
            }

            Member memberObj = metaTreeNode.Properties[0].Value as Member;
            if (memberObj != null)
            {
                MemberElement memberElement = new MemberElement();
                memberElement.UniqueName = memberObj.UniqueName;
                memberElement.Name = memberObj.Name;
                memberElement.Visible = memberObj.Visible;
                if (metaTreeNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                {
                    memberElement.IsSelectedChildMembers = true;
                }
                else
                {
                    memberElement.IsSelectedChildMembers = false;
                }

                memberElement.Properties.Add(new Property(PropertyConstants.Member, memberObj));
                if (metaTreeNode.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode memberMetaTreeNode in metaTreeNode.ChildNodes)
                    {
                        UpdateMemberElement(memberElement, memberMetaTreeNode);
                    }
                }

                return memberElement;
            }

            return null;
        }

        /// <summary>
        /// Updates the hierarchy element.
        /// </summary>
        /// <param name="parentDimensionElement">The parent dimension element.</param>
        /// <param name="metaTreeNodeHierarchy">The meta tree node hierarchy.</param>
        static void UpdateHierarchyElement(DimensionElement parentDimensionElement, MetaTreeNode metaTreeNodeHierarchy)
        {
            Hierarchy hierarchyObj = null;
            MetaTreeNode __mainMetaTreeHierarchyNode = null;
            if (!(metaTreeNodeHierarchy.Properties.Count > 0))
            {
                if (metaTreeNodeHierarchy.NodeType == MetaTreeNodeType.DisplayFolder)
                {
                    if (metaTreeNodeHierarchy.ChildNodes.Count > 0)
                    {
                        __mainMetaTreeHierarchyNode = metaTreeNodeHierarchy.ChildNodes[0];
                        hierarchyObj = __mainMetaTreeHierarchyNode.Properties[0].Value as Hierarchy;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                hierarchyObj = metaTreeNodeHierarchy.Properties[0].Value as Hierarchy;
                __mainMetaTreeHierarchyNode = metaTreeNodeHierarchy;
            }

            if (hierarchyObj != null)
            {
                parentDimensionElement.SetHierarchy(hierarchyObj.Name);
                HierarchyElement hierarchyElement = parentDimensionElement.Hierarchy;
                hierarchyElement.Properties.Add(new Property(PropertyConstants.Hierarchy, hierarchyObj));
                if (metaTreeNodeHierarchy.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode levelMetaTreeNode in __mainMetaTreeHierarchyNode.ChildNodes)
                    {
                        UpdateLevelElement(hierarchyElement, levelMetaTreeNode);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the level element based on the Metatreenode and adds it to the levelElement.
        /// </summary>
        /// <param name="parentHierarchyElement">The parent hierarchy element.</param>
        /// <param name="metaTreeLevelNode">The meta tree level node.</param>
        static void UpdateLevelElement(HierarchyElement parentHierarchyElement, MetaTreeNode metaTreeLevelNode)
        {
            if (!(metaTreeLevelNode.Properties.Count > 0))
            {
                return;
            }

            Level levelObj = metaTreeLevelNode.Properties[0].Value as Level;
            if (levelObj != null)
            {
                parentHierarchyElement.Add(levelObj.Name);
                LevelElement levelElement = parentHierarchyElement.LevelElements[levelObj.Name];
                levelElement.Properties.Add(new Property(PropertyConstants.Level, levelObj));
                if (metaTreeLevelNode.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode memberMetaTreeNode in metaTreeLevelNode.ChildNodes)
                    {
                        UpdateMemberElement(levelElement, memberMetaTreeNode);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the named set element from Metatreenode.
        /// </summary>
        /// <param name="namedSetElement">The named set element.</param>
        /// <param name="metaTreeNamedSetNode">The meta tree named set node.</param>
        static void UpdateNamedSetElement(NamedSetElement namedSetElement, MetaTreeNode metaTreeNamedSetNode)
        {
            if (!(metaTreeNamedSetNode.Properties.Count > 0))
            {
                return;
            }

            NamedSet namedSetObj = metaTreeNamedSetNode.Properties.FindByName(PropertyConstants.NamedSet).Value as NamedSet;
            if (namedSetObj != null)
            {
                namedSetElement.Name = namedSetObj.Name;
                namedSetElement.DimensionName = namedSetObj.ParentDimensionName;
                namedSetElement.Properties.Add(new Property(PropertyConstants.NamedSet, namedSetObj));
                Dimension dimensionObj = metaTreeNamedSetNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                if (dimensionObj != null)
                {
                    namedSetElement.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                }
            }
        }

        /// <summary>
        /// Updates the kpi elements.
        /// </summary>
        /// <param name="kpiElements">The kpi elements.</param>
        /// <param name="metaTreeNodeMeasures">The meta tree node measures.</param>
        /// <param name="isCurrentMember">if set to <c>true</c> [is current member].</param>
        static void UpdateKpiElements(KpiElements kpiElements, MetaTreeNode metaTreeNodeMeasures, bool isCurrentMember)
        {
            if (metaTreeNodeMeasures.NodeType == MetaTreeNodeType.DisplayFolder
                || metaTreeNodeMeasures.NodeType == MetaTreeNodeType.KPI_ROOT)
            {
                if (metaTreeNodeMeasures.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode measureNode in metaTreeNodeMeasures.ChildNodes)
                    {
                        UpdateKpiElements(kpiElements, measureNode, isCurrentMember);
                    }
                }
                else
                {
                    kpiElements.Elements.Clear();
                }
            }
            else if (metaTreeNodeMeasures.NodeType == MetaTreeNodeType.KPI)
            {
                if (metaTreeNodeMeasures.Properties.Count > 0)
                {
                    Kpi kpiObj = metaTreeNodeMeasures.Properties[0].Value as Kpi;
                    if (isCurrentMember)
                    {
                        if (kpiObj != null && (metaTreeNodeMeasures.IsSelected == true || metaTreeNodeMeasures.IsSelected == null))
                        {
                            if (kpiElements.Elements.FindKpiByName(kpiObj.Name) != null)
                            {
                            }
                        }
                    }
                    else
                    {
                        if (kpiObj != null)
                        {
                            kpiElements.Add(kpiObj.Name);
                        }
                    }

                }
            }
        }

        static void SynchronizeKpiElements(KpiElements kpiElements, MetaTreeNode metaTreeNodeKpis)
        {
            if (metaTreeNodeKpis.NodeType == MetaTreeNodeType.KPI_ROOT)
            {
                if (metaTreeNodeKpis.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode kpiNode in metaTreeNodeKpis.ChildNodes)
                    {
                        SynchronizeKpiElements(kpiElements, kpiNode);
                    }
                }
            }
            else if (metaTreeNodeKpis.NodeType == MetaTreeNodeType.KPI)
            {
                KpiElement kpiElement = GetKpiElement(metaTreeNodeKpis);
                if (kpiElement != null)
                {
                    if (kpiElement.ShowKPIValue || kpiElement.ShowKPIGoal || kpiElement.ShowKPIStatus || kpiElement.ShowKPITrend)
                        kpiElements.Elements.Add(kpiElement);
                }
            }
        }

        static KpiElement GetKpiElement(MetaTreeNode metaTreeNode)
        {
            if (metaTreeNode.NodeType == MetaTreeNodeType.KPI)
            {
                KpiElement kpiElement = new KpiElement();
                kpiElement.Name = metaTreeNode.Name;
                kpiElement.ShowKPIValue = CheckKpiIsSelected(metaTreeNode, MetaTreeNodeType.KPI_Value);
                kpiElement.ShowKPIGoal = CheckKpiIsSelected(metaTreeNode, MetaTreeNodeType.KPI_Goal);
                kpiElement.ShowKPIStatus = CheckKpiIsSelected(metaTreeNode, MetaTreeNodeType.KPI_Status);
                kpiElement.ShowKPITrend = CheckKpiIsSelected(metaTreeNode, MetaTreeNodeType.KPI_Trend);
                return kpiElement;
            }
            return null;
        }

        static bool CheckKpiIsSelected(MetaTreeNode metaTreeNode, MetaTreeNodeType nodeType)
        {
            var nodeItem = metaTreeNode.ChildNodes.Where(i => i.NodeType == nodeType).Select(i => i);
            if (nodeItem != null && nodeItem.Count() > 0)
            {
                MetaTreeNode node = nodeItem.First();
                if (node.IsSelected == true)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the measure elements from Metatreenode and adds it to the MeasureElements collection.
        /// </summary>
        /// <param name="measureElements">The measure elements.</param>
        /// <param name="metaTreeNodeMeasures">The meta tree node measures.</param>
        static void SynchronizeMeasureElements(MeasureElements measureElements, MetaTreeNode metaTreeNodeMeasures)
        {
            if (metaTreeNodeMeasures.NodeType == MetaTreeNodeType.DisplayFolder
                || metaTreeNodeMeasures.NodeType == MetaTreeNodeType.MeasureGroup)
            {
                if (metaTreeNodeMeasures.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode measureNode in metaTreeNodeMeasures.ChildNodes)
                    {
                        SynchronizeMeasureElements(measureElements, measureNode);
                    }
                }
                else
                {
                    measureElements.Elements.Clear();
                }
            }
            else if (metaTreeNodeMeasures.NodeType == MetaTreeNodeType.Measure)
            {
                if (metaTreeNodeMeasures.Properties.Count > 0)
                {
                    if (metaTreeNodeMeasures.IsSelected == true)
                    {
                        Measure measureObj = metaTreeNodeMeasures.Properties[0].Value as Measure;
                        if (measureObj != null)
                        {
                            if (measureElements.Elements.FindMeasureByName(measureObj.UniqueName) == null)
                            {
                                measureElements.Add(measureObj.Name);
                            }
                        }
                    }
                    else
                    {
                        Measure measureObj = metaTreeNodeMeasures.Properties[0].Value as Measure;
                        if (measureObj != null)
                        {
                            if (measureElements.Elements.FindMeasureByName(measureObj.UniqueName) == null)
                            {
                                measureElements.ExcludedMeasures.Add(new MeasureElement() { Name = measureObj.Name });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the measure elements from Metatreenode and adds it to the MeasureElements collection.
        /// </summary>
        /// <param name="namedSetElement">The named set element.</param>
        /// <param name="metaTreeNodeNamedSet">The meta tree node named set.</param>
        static void SynchronizeNamedSetElement(NamedSetElement namedSetElement, MetaTreeNode metaTreeNodeNamedSet)
        {
            if (metaTreeNodeNamedSet.NodeType == MetaTreeNodeType.NamedSet)
            {
                if (metaTreeNodeNamedSet.Properties.Count > 0)
                {
                    NamedSet namedSetObj = metaTreeNodeNamedSet.Properties.FindByName(PropertyConstants.NamedSet).Value as NamedSet;
                    if (namedSetObj != null)
                    {
                        namedSetElement.Name = namedSetObj.Name;
                        namedSetElement.DimensionName = namedSetObj.ParentDimensionName;
                        namedSetElement.ParentDimension = new DimensionElement();
                        namedSetElement.ParentDimension.Name = namedSetObj.ParentDimensionName;
                    }
                }
            }
        }

        static void SynchronizeCalculatedMembers(CalculatedMember calculatedMemberElement, MetaTreeNode metaTreeNodeCalcMember)
        {
            if (metaTreeNodeCalcMember.NodeType == MetaTreeNodeType.CalculatedMember)
            {
                CalculatedMember calcMemberObj = metaTreeNodeCalcMember.Properties[0].Value as CalculatedMember;
                if (calcMemberObj != null)
                {
                    calculatedMemberElement.Name = calcMemberObj.Name;
                    calculatedMemberElement.UniqueName = calcMemberObj.UniqueName;
                    calculatedMemberElement.Type = calcMemberObj.Type;
                    calculatedMemberElement.CustomExpression = calcMemberObj.CustomExpression;
                    calculatedMemberElement.Expression = calcMemberObj.Expression;
                    calculatedMemberElement.FormatString = calcMemberObj.FormatString;
                    calculatedMemberElement.Properties = calcMemberObj.Properties;
                    calculatedMemberElement.Visible = calcMemberObj.Visible;
                }
            }            
        }
        static void SynchronizeVirtualKpiElement(VirtualKpiElement virtualKpiElements, MetaTreeNode metaTreeNode)
        {
            if (metaTreeNode.NodeType == MetaTreeNodeType.VirtualKPIMember)
            {
                foreach (MetaTreeNode mtnode in metaTreeNode.ChildNodes)
                {
                    if (mtnode.NodeType == MetaTreeNodeType.VirtualKPIMember)
                    {
                        SynchronizeVirtualKpiElement(virtualKpiElements, mtnode);
                    }
                    else if (mtnode.NodeType == MetaTreeNodeType.VirtualKPI_Value || mtnode.NodeType == MetaTreeNodeType.VirtualKPI_Goal || mtnode.NodeType == MetaTreeNodeType.VirtualKPI_Status || mtnode.NodeType == MetaTreeNodeType.VirtualKPI_Trend)
                    {
                        VirtualKpiElement virtualKpi = mtnode.Properties[0].Value as VirtualKpiElement;
                        if (virtualKpi != null)
                        {
                            virtualKpiElements.ShowVirtualKPIValue = CheckKpiIsSelected(metaTreeNode, MetaTreeNodeType.VirtualKPI_Value);
                            virtualKpiElements.ShowVirtualKPIGoal = CheckKpiIsSelected(metaTreeNode, MetaTreeNodeType.VirtualKPI_Goal);
                            virtualKpiElements.ShowVirtualKPIStatus = CheckKpiIsSelected(metaTreeNode, MetaTreeNodeType.VirtualKPI_Status);
                            virtualKpiElements.ShowVirtualKPITrend = CheckKpiIsSelected(metaTreeNode, MetaTreeNodeType.VirtualKPI_Trend);
                            virtualKpiElements.ElementName = virtualKpi.ElementName;
                            virtualKpiElements.Expression = virtualKpi.Expression;
                            virtualKpiElements.Name = virtualKpi.Name;
                            virtualKpiElements.Properties = virtualKpi.Properties;
                            virtualKpiElements.StatusGraphic = virtualKpi.StatusGraphic;
                            virtualKpiElements.TrendGraphic = virtualKpi.TrendGraphic;
                            virtualKpiElements.Visible = virtualKpi.Visible;
                            if (virtualKpiElements.ShowVirtualKPIValue)
                                virtualKpiElements.KpiValueExpression = virtualKpi.virtualKpiValue;
                            else if(virtualKpi.KpiValueExpression!=null)
                                virtualKpiElements.KpiValueExpression = "";
                            if (virtualKpiElements.ShowVirtualKPIGoal)
                                virtualKpiElements.KpiGoalExpression = virtualKpi.virtualKpiGoal;
                            else if(virtualKpi.KpiGoalExpression!=null)
                                virtualKpiElements.KpiGoalExpression = "";
                            if (virtualKpiElements.ShowVirtualKPIStatus)
                                virtualKpiElements.KpiStatusExpression = virtualKpi.virtualKpiStatus;
                            else if(virtualKpi.KpiStatusExpression!=null)
                                virtualKpiElements.KpiStatusExpression = "";
                            if (virtualKpiElements.ShowVirtualKPITrend)
                                virtualKpiElements.KpiTrendExpression = virtualKpi.virtualKpiTrend;
                            else if(virtualKpi.KpiTrendExpression!=null)
                                virtualKpiElements.KpiTrendExpression = "";
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// Updates the child member element into the Parent Level Element.
        /// </summary>
        /// <param name="parentLevelElement">The parent level element.</param>
        /// <param name="metaTreeMemberNode">The meta tree member node.</param>
        static void UpdateMemberElement(LevelElement parentLevelElement, MetaTreeNode metaTreeMemberNode)
        {
            if (!(metaTreeMemberNode.Properties.Count > 0))
            {
                return;
            }

            Member memberObj = metaTreeMemberNode.Properties[0].Value as Member;
            if (memberObj != null)
            {
                parentLevelElement.Add(memberObj.Name, memberObj.UniqueName);
                MemberElement memberElement = parentLevelElement.MemberElements[memberObj.UniqueName];
                if (metaTreeMemberNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                {
                    memberElement.IsSelectedChildMembers = true;
                }
                else
                {
                    memberElement.IsSelectedChildMembers = false;
                }

                memberElement.Properties.Add(new Property(PropertyConstants.Member, memberObj));
                if (metaTreeMemberNode.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode memberMetaTreeNode in metaTreeMemberNode.ChildNodes)
                    {
                        UpdateMemberElement(memberElement, memberMetaTreeNode);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the member element.
        /// </summary>
        /// <param name="excludedDimensionElement">The excluded dimension element.</param>
        /// <param name="metaTreeMemberNode">The metatree member node.</param>
        /// <param name="hierarchyObj">The hierarchy object for updating the Excluded dimension</param>
        /// <param name="levelObj">The level object for updating the Excluded dimension</param>
        /// <returns>Dimension element along with excluded members</returns>
        static DimensionElement UpdateMemberElement(ref DimensionElement excludedDimensionElement, MetaTreeNode metaTreeMemberNode, Hierarchy hierarchyObj, Level levelObj)
        {
            if (metaTreeMemberNode.Properties.Count <= 0)
                return null;
            Member memberObj = metaTreeMemberNode.Properties[0].Value as Member;
            if (string.IsNullOrEmpty(memberObj.ParentCaption))
                memberObj.ParentCaption = metaTreeMemberNode.ParentNode.Caption;
            if (memberObj != null)
            {
                if (metaTreeMemberNode.NodeCheckedType == MetaTreeNodeCheckedType.NoneSelected)
                {
                    excludedDimensionElement = AddMemberToLevel(ref excludedDimensionElement, memberObj, hierarchyObj, levelObj);
                }
                else if (metaTreeMemberNode.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked)
                {
                    RemoveMemberFromDimension(ref excludedDimensionElement, memberObj);
                }
                else if (metaTreeMemberNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                {
                    if (metaTreeMemberNode.ChildNodes.Count > 0)
                    {
                        foreach (MetaTreeNode memberMetaTreeNode in metaTreeMemberNode.ChildNodes)
                        {
                            Member childMemberObj = memberMetaTreeNode.Properties[0].Value as Member;
                            childMemberObj.ParentCaption = metaTreeMemberNode.ParentNode.Caption;
                            if (memberMetaTreeNode.NodeType == MetaTreeNodeType.None)
                            {
                                break;
                            }
                            else if (memberMetaTreeNode.NodeCheckedType == MetaTreeNodeCheckedType.NoneSelected)
                            {
                                excludedDimensionElement = AddMemberToLevel(ref excludedDimensionElement, childMemberObj, hierarchyObj, levelObj);
                            }
                            else if (memberMetaTreeNode.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked)
                            {
                                RemoveMemberFromDimension(ref excludedDimensionElement, childMemberObj);
                            }
                            else if (memberMetaTreeNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                            {
                                excludedDimensionElement = UpdateMemberElement(ref excludedDimensionElement, memberMetaTreeNode, hierarchyObj, levelObj);
                            }
                        }
                    }
                }
            }

            return excludedDimensionElement;
        }

        static void RemoveMemberFromDimension(ref DimensionElement excludedDimensionElement, Member memberObj)
        {
            if (excludedDimensionElement == null)
            {
                return;
            }

            bool breakFlag = false;
            foreach (LevelElement levelElement in excludedDimensionElement.Hierarchy.LevelElements)
            {
                if (memberObj.LevelUniqueName == levelElement.UniqueName || memberObj.LevelUniqueName.Split('.')[0] + "." + memberObj.LevelUniqueName == levelElement.UniqueName)
                {
                    int memberCount = levelElement.MemberElements.Count;
                    for (int i = 0; i < memberCount; i++)
                    {
                        MemberElement memberElement = levelElement.MemberElements[i];
                        if (memberElement.UniqueName == memberObj.UniqueName ||
                            memberElement.UniqueName == memberObj.CustomUniqueName)
                        {
                            levelElement.MemberElements.Remove(memberElement);
                            breakFlag = true;
                            break;
                        }
                    }
                    if (breakFlag)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the child member element into the Member Element of the Parent Level Element.
        /// </summary>
        /// <param name="parentMemberElement">The parent member element.</param>
        /// <param name="metaTreeMemberNode">The meta tree member node.</param>
        static void UpdateMemberElement(MemberElement parentMemberElement, MetaTreeNode metaTreeMemberNode)
        {
            if (!(metaTreeMemberNode.Properties.Count > 0) || metaTreeMemberNode.NodeCheckedType == MetaTreeNodeCheckedType.NoneSelected)
            {
                return;
            }

            Member memberObj = metaTreeMemberNode.Properties[0].Value as Member;
            if (memberObj != null)
            {
                parentMemberElement.Add(new MemberElement { Name = memberObj.Name, UniqueName = memberObj.UniqueName });
                MemberElement memberElement = parentMemberElement.ChildMemberElements[memberObj.UniqueName];
                //memberElement.UniqueName = memberObj.UniqueName;
                //memberElement.Name = memberObj.Name;
                if (metaTreeMemberNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                {
                    memberElement.IsSelectedChildMembers = true;
                }
                else
                {
                    memberElement.IsSelectedChildMembers = false;
                }

                memberElement.Properties.Add(new Property(PropertyConstants.Member, memberObj));
                if (metaTreeMemberNode.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode memberMetaTreeNode in metaTreeMemberNode.ChildNodes)
                    {
                        UpdateMemberElement(memberElement, memberMetaTreeNode);
                    }
                }
            }
        }
        #endregion
    }
}