#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using BASE = Syncfusion.Olap.Data;
using SLBASE = Syncfusion.OlapSilverlight.Data;
using Syncfusion.Olap.Data;

namespace Syncfusion.OlapSilverlight.Wrapper
{
   internal class CubeSchemaWrapper
    {
        internal static SLBASE.CubeInfoCollection GetCubes(BASE.CubeInfoCollection cubes)
        {
            SLBASE.CubeInfoCollection cubeCollestion = null;
            if(cubes != null)
            {
                cubeCollestion = new SLBASE.CubeInfoCollection();
                foreach (BASE.CubeInfo cube in cubes)
                {
                    cubeCollestion.Add(new SLBASE.CubeInfo(cube.Name, cube.Caption, cube.Description));
                }  
            }
            return cubeCollestion;
        }

        internal static SLBASE.CubeSchema GetCubeSchema(BASE.CubeSchema cubeSchema)
        {
            SLBASE.CubeSchema m_cubeSchema = null;
            if (cubeSchema != null)
            {
                m_cubeSchema = new SLBASE.CubeSchema();
                m_cubeSchema.CubeInfo = new SLBASE.CubeInfo(cubeSchema.CubeInfo.Name, cubeSchema.CubeInfo.Caption, cubeSchema.CubeInfo.Description);
                m_cubeSchema.Dimensions = GetDimensionCollection(cubeSchema.Dimensions, m_cubeSchema);
                m_cubeSchema.Kpis = GetKpis(cubeSchema.Kpis, m_cubeSchema);
                m_cubeSchema.Measures = GetMeasures(cubeSchema.Measures, m_cubeSchema);
                m_cubeSchema.NamedSets = GetNamedSets(cubeSchema.NamedSets, m_cubeSchema);
            }
           return m_cubeSchema;
        }

        internal static SLBASE.DimensionCollection GetDimensionCollection(DimensionCollection dimensionCollection, SLBASE.CubeSchema m_cubeSchema)
        {
            if (dimensionCollection != null)
            {
                SLBASE.DimensionCollection m_dimensionCollection = new SLBASE.DimensionCollection(m_cubeSchema);
                foreach (BASE.Dimension dimension in dimensionCollection)
                {
                    m_dimensionCollection.Insert(m_dimensionCollection.Count,GetDimension(dimension, m_cubeSchema));
                }
                return m_dimensionCollection;
            }
            return null;
        }

        internal static SLBASE.Dimension GetDimension(Dimension dimension, SLBASE.CubeSchema m_cubeSchema)
        {
            SLBASE.Dimension m_dimension = new SLBASE.Dimension();
                m_dimension.Caption = dimension.Caption;
                m_dimension.Name = dimension.Name;
                m_dimension.UniqueName = dimension.UniqueName;
                m_dimension.ParentCubeSchema = m_cubeSchema;
                m_dimension.DefaultHierarchyName = dimension.DefaultHierarchyName;
                m_dimension.Description = dimension.Description;
                m_dimension.Hierarchies = GetHierarchyCollection(dimension.Hierarchies, m_dimension);
                m_dimension.DimensionType = (SLBASE.DimensionTypeEnum)Enum.Parse(typeof(BASE.DimensionTypeEnum),dimension.DimensionType.ToString());
                m_dimension.Visible = dimension.Visible;
            return m_dimension;
        }

        internal static SLBASE.HierarchyCollection GetHierarchyCollection(HierarchyCollection hierarchyCollection, SLBASE.Dimension m_dimensioin)
        {
            if (hierarchyCollection != null)
            {
                SLBASE.HierarchyCollection m_hierarchys = new SLBASE.HierarchyCollection(m_dimensioin);
                foreach (BASE.Hierarchy hierarchy in hierarchyCollection)
                {
                    m_hierarchys.Add(GerHierarchy(hierarchy, m_dimensioin));
                }
                return m_hierarchys;
            }
            return null;
        }

        internal static SLBASE.Hierarchy GerHierarchy(Hierarchy hierarchy, SLBASE.Dimension m_dimensioin)
        {
            SLBASE.Hierarchy m_hierarchy = new SLBASE.Hierarchy();
                m_hierarchy.Caption = hierarchy.Caption;
                m_hierarchy.Name = hierarchy.Name;
                m_hierarchy.UniqueName = hierarchy.UniqueName;
                m_hierarchy.DefaultLevelName = hierarchy.DefaultLevelName;
                m_hierarchy.DefaultLevelUniqueName = hierarchy.DefaultLevelUniqueName;
                m_hierarchy.DefaultMemberUniqueName = hierarchy.DefaultMemberUniqueName;
                m_hierarchy.Description = hierarchy.Description;
                m_hierarchy.DisplayFolder = hierarchy.DisplayFolder;
                m_hierarchy.IsAttributeHierarchy = hierarchy.IsAttributeHierarchy;
                m_hierarchy.ParentDimension = m_dimensioin;
                m_hierarchy.Levels = GetLevelCollection(hierarchy.Levels, m_hierarchy);
           return m_hierarchy;
        }

        internal static SLBASE.LevelCollection GetLevelCollection(LevelCollection levelCollection, SLBASE.Hierarchy m_hierarchy)
        {
            if (levelCollection != null)
            {
                SLBASE.LevelCollection m_levels = new SLBASE.LevelCollection(m_hierarchy);
                foreach (BASE.Level level in levelCollection)
                {
                    m_levels.Add(GetLevel(level, m_hierarchy));
                }

                return m_levels;
            }
            return null;
        }

        internal static SLBASE.Level GetLevel(Level level, SLBASE.Hierarchy m_hierarchy)
        {
            SLBASE.Level m_level = new SLBASE.Level();

            m_level.Caption = level.Caption;
            m_level.Name = level.Name;
            m_level.UniqueName = level.UniqueName;
            m_level.Description = level.Caption;
            m_level.IsMemberLoadedOnDemand = level.IsMemberLoadedOnDemand;
            m_level.LevelDepth = level.LevelDepth;
            m_level.LevelType = (SLBASE.LevelTypeEnum)Enum.Parse(typeof(BASE.LevelTypeEnum), level.LevelType.ToString());
            //m_level.MemberCount = level.MemberCount; //TODO: No need to get level members and its count. We should call by asynchronously.
            m_level.ParentHierarchy = m_hierarchy;
            //m_level.Members = GetMemberCollection(level.Members, m_level);

            return m_level;
        }

        internal static SLBASE.MemberCollection GetMemberCollection(MemberCollection memberCollection, SLBASE.Level m_level, bool getChild)
        { 
            if (memberCollection != null)
            {
                SLBASE.MemberCollection m_members = new SLBASE.MemberCollection(m_level);
                foreach (BASE.Member member in memberCollection)
                {
                    m_members.Add(GetMember(member, m_level, null, getChild));
                }

                return m_members;
            }
            return null;
        }

        internal static SLBASE.Member GetMember(Member member, SLBASE.Level m_patentLevel, SLBASE.Member m_parentMember, bool getChild)
        {
            SLBASE.Member m_member = new SLBASE.Member();
            m_member.Caption = member.Caption;
            m_member.Name = member.Name;
            m_member.UniqueName = member.UniqueName;
            m_member.Description = member.Description;
            m_member.DrilledDown = member.DrilledDown;
            m_member.ParentHierarchy = member.ParentHierarchy;
            m_member.HasChildMembers = member.HasChildMembers;
            m_member.IsMemberLoadedOnDemand = member.IsMemberLoadedOnDemand;
            m_member.KPIStatusGraphic = member.KPIStatusGraphic;
            m_member.KPIType = (SLBASE.KpiTypeEnum)member.KPIType;
            m_member.LevelDepth = member.LevelDepth;
            m_member.LevelUniqueName = member.LevelUniqueName;
            m_member.KPITrendGraphic = member.KPITrendGraphic;
            m_member.Visible = member.Visible;
            m_member.Type = (SLBASE.MemberTypeEnum)member.Type;//GetEnumType(member.Type);
            m_member.ParentCaption = member.ParentCaption;
            m_member.ParentUniqueName = member.ParentUniqueName;
            if (member.ParentLevel != null && m_patentLevel != null)
            {
                m_member.ParentLevel = m_patentLevel;
            }
            //if (member.ParentMember != null && m_parentMember != null)
            //{
            //   // m_member.ParentMember = m_parentMember;
            //}
            m_member.Kpi_Name = GetKpiFromProperties(member.Properties);
            m_member.MemberProperties = GetMemberProperties(member.MemberProperties);

            if (m_member.HasChildMembers && getChild)
            {
                m_member.ChildMembers.AddRange(GetMemberCollection(member.ChildMembers, null, false));
            }

            return m_member;
        }

        private static SLBASE.PropertyCollection GetMemberProperties(PropertyCollection propertyCollection)
        {
            if (propertyCollection != null)
            {
                SLBASE.PropertyCollection m_PropertyCollection = new SLBASE.PropertyCollection();

                foreach (Property property in propertyCollection)
                {
                    m_PropertyCollection.Add(new SLBASE.Property { Name = property.Name, Value = property.Value });
                }

                return m_PropertyCollection;
            }
            return null;
        }

        private static string GetKpiFromProperties(PropertyCollection propertyCollection)
        {
            if (propertyCollection != null)
            {
                string kpiName = string.Empty;
                foreach (Property property in propertyCollection)
                {
                    if (property.Name == BASE.PropertyConstants.KPI)
                    {
                        Syncfusion.Olap.Data.Property kpi_Property = (Syncfusion.Olap.Data.Property)property.Value;
                        kpiName = kpi_Property.Value.ToString();
                    }
                }
                return kpiName;
            }
            return string.Empty;
        }

        private static SLBASE.MemberTypeEnum GetEnumType(MemberTypeEnum memberTypeEnum)
        {
            if (memberTypeEnum == MemberTypeEnum.All)
            {
                return SLBASE.MemberTypeEnum.All;
            }
            else if (memberTypeEnum == MemberTypeEnum.Formula)
            {
                return SLBASE.MemberTypeEnum.Formula;
            }
            else if (memberTypeEnum == MemberTypeEnum.Measure)
            {
                return SLBASE.MemberTypeEnum.Measure;
            }
            else if (memberTypeEnum == MemberTypeEnum.Regular)
            {
                return SLBASE.MemberTypeEnum.Regular;
            }
            else
            {
                return SLBASE.MemberTypeEnum.Unknown;
            }
        }

        internal static SLBASE.KpiCollection GetKpis(KpiCollection kpiCollection, SLBASE.CubeSchema m_cubeSchema)
        {
            if (kpiCollection != null)
            {
                SLBASE.KpiCollection m_kpiCollection = new SLBASE.KpiCollection();
                foreach (Kpi kpi in kpiCollection)
                {
                    m_kpiCollection.Add(
                        new SLBASE.Kpi
                        {
                            Caption = kpi.Caption,
                            Description = kpi.Description,
                            Name = kpi.Name,
                            UniqueName = kpi.UniqueName,
                            DisplayFolder = kpi.DisplayFolder,
                            StatusGraphic = kpi.StatusGraphic,
                            TrendGraphic = kpi.TrendGraphic,
                            ParentCubeSchema = m_cubeSchema
                        });
                }
                return m_kpiCollection;
            }

            return null;
        }

        internal static SLBASE.MeasureCollection GetMeasures(MeasureCollection measures, SLBASE.CubeSchema m_cubeSchema)
        {
            
            if (measures != null)
            {
                SLBASE.MeasureCollection m_measures = new SLBASE.MeasureCollection();
                m_measures = new SLBASE.MeasureCollection();
                foreach (BASE.Measure measure in measures)
                {
                    m_measures.Add(new SLBASE.Measure
                    {
                        Caption= measure.Caption,
                        Description = measure.Description,
                        GroupName = measure.GroupName,
                        DisplayFolder = measure.DisplayFolder,
                        MeasureAggregator = measure.MeasureAggregator,
                        Name = measure.Name,
                        UniqueName = measure.UniqueName,
                        NumericPrecision = measure.NumericPrecision,
                        NumericScale = measure.NumericScale,
                        ParentCubeName = measure.ParentCubeName,
                        ParentCubeSchema = m_cubeSchema,
                        Units = measure.Units,
                        Visible = measure.Visible
                    });                   
                }

                return m_measures;
            }
            return null;
        }

        internal static SLBASE.NamedSetCollection GetNamedSets(NamedSetCollection namedSets, SLBASE.CubeSchema m_cubeSchema)
        {
            if (namedSets != null)
            {
                SLBASE.NamedSetCollection m_namedSets = new SLBASE.NamedSetCollection();

                foreach (BASE.NamedSet namedSet in namedSets)
                {
                    m_namedSets.Add(new SLBASE.NamedSet
                    {
                        Name = namedSet.Name,
                        Description = namedSet.Description,
                        Expression = namedSet.Expression,
                        ParentCubeSchema = m_cubeSchema,
                        ParentDimensionName = namedSet.ParentDimensionName,
                        ParentHierarchyName = namedSet.ParentHierarchyName,
                        DisplayFolder = namedSet.DisplayFolder
                    });
                }
                return m_namedSets;
            }
            return null;
        }
    }
}
