#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using Syncfusion.Olap.Data;

namespace Syncfusion.OlapSilverlight.Wrapper
{
    public class CellSetWrapper
    {
        /// <summary>
        /// Returns Silverlight CellSet Wrapper from Olap.Base CellSet
        /// </summary>
        /// <param name="olapCellSet">The cell set.</param>
        /// <param name="cellCollection">The cell collection.</param>
        /// <returns>A CellSet.</returns>
        internal static Syncfusion.OlapSilverlight.Data.CellSet GetCellSetWrapper(Syncfusion.Olap.Data.CellSet olapCellSet, Syncfusion.Olap.Data.CellCollection cellCollection)
        {
            Syncfusion.OlapSilverlight.Data.CellSet m_CellSet = new Syncfusion.OlapSilverlight.Data.CellSet();

            foreach (Syncfusion.Olap.Data.Axis axis in olapCellSet.Axes)
            {
                Syncfusion.OlapSilverlight.Data.Axis wrapperAxis = new Syncfusion.OlapSilverlight.Data.Axis();
                foreach (Syncfusion.Olap.Data.Tuple tuple in axis.TupleSet)
                {
                    Syncfusion.OlapSilverlight.Data.Tuple wrapperTuple = new Syncfusion.OlapSilverlight.Data.Tuple();
                    foreach (Syncfusion.Olap.Data.Member member in tuple.Members)
                    {
                        Syncfusion.OlapSilverlight.Data.Member m_Member = new Syncfusion.OlapSilverlight.Data.Member();
                        m_Member.Caption = member.Caption;
                        m_Member.Description = member.Description;
                        m_Member.DrilledDown = member.DrilledDown;
                        m_Member.HasChildMembers = member.HasChildMembers;
                        m_Member.ParentHierarchy = member.ParentHierarchy;
                        m_Member.KPIStatusGraphic = member.KPIStatusGraphic;
                        m_Member.KPIType = (Syncfusion.OlapSilverlight.Data.KpiTypeEnum)member.KPIType;
                        m_Member.LevelDepth = member.LevelDepth;
                        m_Member.LevelUniqueName = member.LevelUniqueName;
                        m_Member.Name = member.Name;
                        m_Member.KPITrendGraphic = member.KPITrendGraphic;
                        m_Member.UniqueName = member.UniqueName;
                        m_Member.Visible = member.Visible;
                        m_Member.Type = GetEnumType(member.Type);
                        m_Member.ParentCaption = member.ParentCaption;
                        m_Member.ParentUniqueName = member.ParentUniqueName;
                        m_Member.Kpi_Name = GetKpiFromProperties(member.Properties);
                        m_Member.MemberProperties = GetMemberProperties(member.MemberProperties);

                        wrapperTuple.Members.Add(m_Member);
                        //wrapperTuple.Members.Add(new Syncfusion.OlapSilverlight.Data.Member 
                        //{
                        //    Caption = member.Caption,
                        //    Description = member.Description,
                        //    DrilledDown = member.DrilledDown,
                        //    HasChildMembers = member.HasChildMembers,
                        //    KPIStatusGraphic = member.KPIStatusGraphic,
                        //    KPIType = (Syncfusion.OlapSilverlight.Reports.KpiTypeEnum)member.KPIType,
                        //    LevelDepth = member.LevelDepth,
                        //    LevelUniqueName = member.LevelUniqueName,
                        //    Name = member.Name,
                        //    KPITrendGraphic = member.KPITrendGraphic,
                        //    UniqueName = member.UniqueName,
                        //    Visible = member.Visible,
                        //    Type = GetEnumType(member.Type),
                        //    ParentCaption = member.ParentCaption,
                        //    ParentUniqueName = member.ParentUniqueName,
                        //    Kpi_Name = GetKpiFromProperties(member.Properties)
                        //});
                    }

                    wrapperTuple.Visible = tuple.Visible;
                    wrapperTuple.OrdinalPosition = tuple.OrdinalPosition;
                    wrapperAxis.TupleSet.Add(wrapperTuple);
                    wrapperAxis.MaxLevel = axis.TupleSet.MaxLevel;
                    wrapperAxis.MinLevel = axis.TupleSet.MinLevel;
                    wrapperAxis.valueSet = axis.TupleSet.valueSet;
                  
                }

                wrapperAxis.Name = axis.Name;
                m_CellSet.Axes.Add(wrapperAxis);
            }

            m_CellSet.ColumnMaxLevel = olapCellSet.ColumnMaxLevel;
            m_CellSet.RowMaxLevel = olapCellSet.RowMaxLevel;
            m_CellSet.CellCollection = new Syncfusion.OlapSilverlight.Data.CellCollection();
            //m_CellSet.CellCollection.ValueEngine.Tag  =  olapCellSet.MemberCellCollection.ValueEngine;
           // m_CellSet.ValueEngine = olapCellSet.ValueEngine;

            foreach (var item in cellCollection)
            {
                List<Syncfusion.OlapSilverlight.Data.Cell> slCell = new List<Syncfusion.OlapSilverlight.Data.Cell>();
                foreach (var item1 in item)
                {
                    Syncfusion.OlapSilverlight.Data.Cell cel = new Syncfusion.OlapSilverlight.Data.Cell();
                    if (item1 != null)
                    {
                        cel.FormattedValue = item1.FormattedValue;
                        cel.FormatString = item1.FormatString;
                        cel.Value = item1.Value; 
                    }
                    slCell.Add(cel);
                }
                m_CellSet.CellCollection.Add(slCell);
                //m_CellSet.CellCollection.Add(item); 
                //cc.Add(new Cell {  FormatedValue = });
                //m_CellSet.CellCollection.Add(new Syncfusion.OlapSilverlight.Data.Cell
                //{
                //    FormatedValue = item.FormatedValue,
                //    FormatString = item.FormatString,
                //    Value = item.Value
                //});
              
            }
            return m_CellSet;
        }

        private static Syncfusion.OlapSilverlight.Data.PropertyCollection GetMemberProperties(PropertyCollection propertyCollection)
        {
            Syncfusion.OlapSilverlight.Data.PropertyCollection memberPropertyCollection = new Syncfusion.OlapSilverlight.Data.PropertyCollection();
            foreach (var item in propertyCollection)
            {
                memberPropertyCollection.Add(new Syncfusion.OlapSilverlight.Data.Property(item.Name, item.Value));
            }
            return memberPropertyCollection;
        }

        private static string GetKpiFromProperties(Syncfusion.Olap.Data.PropertyCollection properties)
        {
            string kpi_Name = string.Empty;
            foreach (Syncfusion.Olap.Data.Property property in properties)
            {
                if (property != null)
                {
                    if (property.Name == PropertyConstants.KPI)
                    {
                        Syncfusion.Olap.Data.Property kpi_Property = (Syncfusion.Olap.Data.Property)property.Value;
                        kpi_Name = kpi_Property.Value.ToString();
                    }
                }
            }
            return kpi_Name;
        }

        /// <summary>
        /// Returns MemberTypeEnum
        /// </summary>
        /// <param name="memberTypeEnum"></param>
        /// <returns></returns>
        private static Syncfusion.OlapSilverlight.Data.MemberTypeEnum GetEnumType(Syncfusion.Olap.Data.MemberTypeEnum memberTypeEnum)
        {
            if (memberTypeEnum == Syncfusion.Olap.Data.MemberTypeEnum.All)
            {
                return Syncfusion.OlapSilverlight.Data.MemberTypeEnum.All;
            }

            else if (memberTypeEnum == Syncfusion.Olap.Data.MemberTypeEnum.Formula)
            {
                return Syncfusion.OlapSilverlight.Data.MemberTypeEnum.Formula;
            }

            else if (memberTypeEnum == Syncfusion.Olap.Data.MemberTypeEnum.Measure)
            {
                return Syncfusion.OlapSilverlight.Data.MemberTypeEnum.Measure;
            }

            else if (memberTypeEnum == Syncfusion.Olap.Data.MemberTypeEnum.Regular)
            {
                return Syncfusion.OlapSilverlight.Data.MemberTypeEnum.Regular;
            }

            else
                return Syncfusion.OlapSilverlight.Data.MemberTypeEnum.Unknown;
        }
    }
}
