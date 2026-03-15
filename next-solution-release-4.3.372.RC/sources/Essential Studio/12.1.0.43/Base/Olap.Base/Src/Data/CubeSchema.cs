//-------------------------------------------------------------------------------------------------
// <copyright file="CubeSchema.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.DataProvider;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// The CubeDataSchema represents the metadata of the cube. It contains information about cube 
    /// Dimensions and Measures. 
    /// </summary>
    /// <remarks>
    /// The CubeSchema is created in GetCubeSchema method of AdomdProvider, whenever a successfull connection is made.  Extracts
    /// all the informations according to the current connection state.  A CubeSchema is associated only with
    /// AdomdProvider object.
    /// </remarks>
    [Serializable]
    public class CubeSchema : IDisposable
    {
#else
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Common;
using Syncfusion.OlapSilverlight.Manager;

using System.Runtime.Serialization;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// The CubeDataSchema represents the metadata of the cube. It contains information about cube 
    /// Dimensions and Measures. 
    /// </summary>
    /// <remarks>
    /// The CubeSchema is created in GetCubeSchema method of AdomdProvider, whenever a successfull connection is made.  Extracts
    /// all the informations according to the current connection state.  A CubeSchema is associated only with
    /// AdomdProvider object.
    /// </remarks>
    [KnownType(typeof(DimensionCollection))]
    [DataContract]
    public class CubeSchema : IDisposable
    {
#endif
        #region Private Variables
#if !SILVERLIGHT
        private IDataProvider _dataProvider = null;
#endif
        #endregion

        #region Constructor
#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="CubeSchema"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public CubeSchema(IDataProvider dataProvider)
        {
            DataProvider = dataProvider;
            this.Dimensions = new DimensionCollection(this);
            this.Measures = new MeasureCollection(this);
            this.Kpis = new KpiCollection(this);
            this.NamedSets = new NamedSetCollection(this);
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="CubeSchema"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public CubeSchema()
        {
            //DataProvider = dataProvider;
            this.Dimensions = new DimensionCollection(this);
            this.Measures = new MeasureCollection(this);
            this.Kpis = new KpiCollection(this);
            this.NamedSets = new NamedSetCollection(this);
        }
#endif
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the cube info.
        /// </summary>
        /// <value>A CubeInfo object represents multidimensional cube information. </value>
        [Description("Gets or sets the cube info."), DefaultValue((string)null)]
#if SILVERLIGHT
        [DataMember]
#endif
        public CubeInfo CubeInfo { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>
        /// <value>The data provider used to retrive the data schema.</value>
        [Description("Gets or sets the data provider."), DefaultValue((string)null)]
        public IDataProvider DataProvider
        {
            get
            {
                return _dataProvider;
            }

            set
            {
                if (_dataProvider != value)
                {
                    _dataProvider = value;
                }
            }
        }
#else
        [DataMember]
        public object DataProvider
        {
            get;
            set;
        }
#endif
        /// <summary>
        /// Gets or sets the Dimension collection.
        /// </summary>
        /// <value>A Collection of Dimension objects.</value>
        [Description("Gets or sets the Dimension collection."), DefaultValue((string)null)]
#if SILVERLIGHT
        [DataMember]
#endif
        public DimensionCollection Dimensions { get; set; }

        /// <summary>
        /// Gets or sets the KPI collection.
        /// </summary>
        /// <value>A Collection of KPI objects.</value>
        [Description("Gets or sets the KPI collection."), DefaultValue((string)null)]
#if SILVERLIGHT
        [DataMember]
#endif
        public KpiCollection Kpis { get; set; }

        /// <summary>
        /// Gets or sets the measures.
        /// </summary>
        /// <value>A Collection of Measure objects</value>
        [DefaultValue((string)null), Description("Gets or sets the measures.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public MeasureCollection Measures { get; set; }

        /// <summary>
        /// Gets or sets the NamedSet collection.
        /// </summary>
        /// <value>A Collection of NamedSet objects.</value>
        [Description("Gets or sets the NamedSet collection."), DefaultValue((string)null)]
#if SILVERLIGHT
        [DataMember]
#endif
        public NamedSetCollection NamedSets { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets the default measure.
        /// </summary>
        /// <returns>Returns the Default Measure Member of the Cube</returns>
        public Member GetDefaultMeasure()
        {
            string commandText = @"SELECT [Measures].DefaultMember dimension properties MEMBER_TYPE, PARENT_UNIQUE_NAME ON 0 , {} ON 1 FROM " + Utils.QuoteIdentifier(CubeInfo.Name);
            CellSet defaultMeasureCellset = this.DataProvider.ExecuteCellSet(commandText, false, true, null);
            if (defaultMeasureCellset.Axes.Count == 2)
            {
                if (defaultMeasureCellset.Axes[0].TupleSet.Count > 0)
                {
                    if (defaultMeasureCellset.Axes[0].TupleSet[0].Members.Count > 0)
                    {
                        Member memberObj = defaultMeasureCellset.Axes[0].TupleSet[0].Members[0];
                        return memberObj;
                    }
                }
            }

            return null;
        }
#endif
        /// <summary>
        /// Gets the Dimension by it unique name
        /// </summary>
        /// <param name="uniqueName">Dimension Unique name.</param>
        /// <returns>Returns the dimension object</returns>
        public Dimension GetDimensionByUniqueName(string uniqueName)
        {
            if (uniqueName.Length > 0)
            {
                foreach (Dimension dimension in this.Dimensions)
                {
                    if (dimension.UniqueName.ToUpper() == uniqueName.ToUpper())
                    {
                        return dimension;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the Hierarchy by it unique name
        /// </summary>
        /// <param name="uniqueName">Hierarchy Unique name.</param>
        /// <returns>returns the Hierarchy object</returns>
        public Hierarchy GetHierarchyByUniqueName(string uniqueName)
        {
            string[] str_HierarchyUniqueName = uniqueName.Split('.');
            if (str_HierarchyUniqueName.Length > 0)
            {
                foreach (Dimension dimension in this.Dimensions)
                {
                    if (dimension.UniqueName == str_HierarchyUniqueName[0])
                    {
                        foreach (Hierarchy hierarchy in dimension.Hierarchies)
                        {
                            if (hierarchy.UniqueName == uniqueName || hierarchy.UniqueName == str_HierarchyUniqueName[1])
                            {
                                return hierarchy;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the level by its unique name.
        /// </summary>
        /// <param name="uniqueName">Level unique name.</param>
        /// <returns>Level objects if found else it will return a null object.</returns>
        public Level GetLevelByUniqueName(string uniqueName)
        {
            string[] str_levelUniqueName = uniqueName.Split('.');
            if (str_levelUniqueName.Length > 1)
            {
                foreach (Dimension dimension in Dimensions)
                {
                    if (dimension.UniqueName == str_levelUniqueName[0])
                    {
                        foreach (Hierarchy hierarchy in dimension.Hierarchies)
                        {
                            if (hierarchy.UniqueName == str_levelUniqueName[0] + "." + str_levelUniqueName[1] || hierarchy.UniqueName == str_levelUniqueName[0])
                            {
                                foreach (Level level in hierarchy.Levels)
                                {
                                    if (level.UniqueName == uniqueName)
                                    {
                                        return level;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the Member by its unique name.
        /// </summary>
        /// <param name="uniqueName">Member uniquename.</param>
        /// <param name="isLoadedMembersOnly">if set to <c>true</c> [is loaded members only].</param>
        /// <returns>returns the Member Object</returns>
        public Member GetMemberByUniqueName(string uniqueName, bool isLoadedMembersOnly)
        {
            return this.GetMember(uniqueName, isLoadedMembersOnly);
        }

        /// <summary>
        /// Gets the NamedSet by its unique name
        /// </summary>
        /// <param name="uniqueName">NamedSets Unique name.</param>
        /// <returns>Returns the namedSet object</returns>
        public NamedSet GetNamedSetByUniqueName(string uniqueName)
        {
            if (uniqueName.Length > 0)
            {
                foreach (NamedSet namedSet in this.NamedSets)
                {
                    if (namedSet.UniqueName == uniqueName)
                    {
                        return namedSet;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the time dimension.
        /// </summary>
        /// <returns>Returns the Time Dimension of the Current Cube</returns>
        public Dimension GetTimeDimension()
        {
            var timeDimensionLevel = this.Dimensions.Where(d => d.DimensionType == DimensionTypeEnum.Time).Select(h => h);
            if (timeDimensionLevel.Count() > 0)
            {
                Dimension dimension = timeDimensionLevel.First();
                return dimension;
            }

            return null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the member.
        /// </summary>
        /// <param name="memberUniqueName">Name of the member unique.</param>
        /// <param name="isLoadedMemberOnly">if set to <c>true</c> [is loaded member only].</param>
        /// <returns>Returns the matching member from the Dimensions</returns>
        Member GetMember(string memberUniqueName, bool isLoadedMemberOnly)
        {
            string[] str_member = memberUniqueName.Split('.');
            if (str_member.Length > 0)
            {
                foreach (Dimension dimension in this.Dimensions)
                {
                    if (dimension.UniqueName == str_member[0])
                    {
                        foreach (Hierarchy hierarchy in dimension.Hierarchies)
                        {
                            if (hierarchy.UniqueName == str_member[0] + "." + str_member[1])
                            {
                                foreach (Level level in hierarchy.Levels)
                                {
#if SILVERLIGHT
                                    if (level.UniqueName == str_member[0] + "." + str_member[1] + "." + str_member[2])
                                    {
#endif
                                    if (level.IsMemberLoadedOnDemand || !isLoadedMemberOnly)
                                    {
                                        foreach (Member member in level.Members)
                                        {
                                            if (member.UniqueName == memberUniqueName || member.CustomUniqueName == memberUniqueName)
                                            {
                                                return member;
                                            }
                                        }
                                    }
#if SILVERLIGHT
                                    break;
                                    }                                   
#endif
                                }
                                break;
                            }                           
                        }
                        break;
                    }

                }
            }

            return null;
        }


        /// <summary>
        /// Gets the member by unique name for non SSAS.
        /// </summary>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <returns>A member object of type <see cref="Member"/>.</returns>
        public Member GetMemberByUniqueNameForNonSSAS(List<string> uniqueName) ////For NonSSAS data
        {
            string[] str_member = uniqueName[0].Split('.');
            foreach (Dimension dimension in this.Dimensions)
            {
                if (dimension.UniqueName == str_member[0])
                {
                    foreach (Hierarchy hierarchy in dimension.Hierarchies)
                    {
                        if (hierarchy.UniqueName == str_member[0])
                        {
                            foreach (Level level in hierarchy.Levels)
                            {
                                if (level.UniqueName == str_member[0] + "." + str_member[1])
                                {
                                    if (level.Members != null)
                                    {
                                        foreach (Member member in level.Members)
                                        {
                                            string memberUniqueName = str_member[0];
                                            for (int i = 2; i < str_member.Count(); i++)
                                            {
                                                memberUniqueName += "." + str_member[i];
                                            }

                                            if (member.UniqueName == memberUniqueName)
                                            {
                                                if (member.ChildMembers != null && member.ChildMembers.Count > 0)
                                                {
                                                    return GetMemberForNonSSAS(uniqueName, member, 1);
                                                }
                                                return member;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return null;
        }

        private Member GetMemberForNonSSAS(List<string> uniqueName, Member member, int depth)
        {
            if (depth <= uniqueName.Count - 1)
            {
                string[] str_member = uniqueName[depth].Split('.');

                foreach (Member child in member.ChildMembers)
                {
                    ////Code for NonSSAS : Find UniqueName 
                    string memberUniqueName = str_member[0];
                    for (int j = 2; j < str_member.Length; j++)
                    {
                        memberUniqueName += "." + str_member[j];
                    }////End of NonSSAS UniqueName identification

                    if (child.UniqueName == memberUniqueName)
                    {
                        if (child.ChildMembers != null && child.ChildMembers.Count > 0)
                        {
                            return GetMemberForNonSSAS(uniqueName, child, depth + 1);
                        }
                        return child;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Gets the name of the member by unique.
        /// </summary>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <returns>A member object of type <see cref="Member"/>.</returns>
        public Member GetMemberByUniqueName(List<string> uniqueName)
        {
            string[] str_member = uniqueName[0].Split('.');
            foreach (Dimension dimension in this.Dimensions)
            {
                if (dimension.UniqueName == str_member[0])
                {
                    foreach (Hierarchy hierarchy in dimension.Hierarchies)
                    {
                        if (hierarchy.UniqueName == str_member[0] + "." + str_member[1])
                        {
#if SILVERLIGHT
                             if(hierarchy.Levels != null)
#endif
                            foreach (Level level in hierarchy.Levels)
                            {
                                if (level.UniqueName == str_member[0] + "." + str_member[1] + "." + str_member[2])
                                {
#if SILVERLIGHT
                                    if(level.Members != null)
#endif
                                    foreach (Member member in level.Members)
                                    {
                                        if (member.UniqueName == uniqueName[0] || member.CustomUniqueName == uniqueName[0])
                                        {
                                            if (member.ChildMembers != null && member.ChildMembers.Count > 0)
                                            {
                                                return GetMember(uniqueName,member,1);
                                            }
                                            return member;
                                        }
                                    }
                                }
                                else if (level.Members != null)
                                {
                                    foreach (var item in level.Members)
                                    {
                                        if (item.UniqueName == str_member[0] + "." + str_member[1] + "." + str_member[2])
                                        {
                                            foreach (Member member in level.Members)
                                            {
                                                if (member.UniqueName == uniqueName[0] || member.CustomUniqueName == uniqueName[0])
                                                {
                                                    if (member.ChildMembers != null && member.ChildMembers.Count > 0)
                                                    {
                                                        return GetMember(uniqueName, member, 1);
                                                    }
                                                    return member;
                                                }
                                            }
                                        } 
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return null;
        }

        private Member GetMember(List<string> uniqueName, object ele, int depth)
        {
            if (depth <= uniqueName.Count - 1)
            {
                int i = depth;
                if (ele is Member)
                {
                    Member members = ele as Member;
                    foreach (Member child in members.ChildMembers)
                    {
                        if (child.UniqueName == uniqueName[i] || members.CustomUniqueName == uniqueName[i])
                        {
                            if (child.ChildMembers != null && child.ChildMembers.Count > 0)
                            {
                                return GetMember(uniqueName, child, depth + 1);
                            }
                            return child;
                        }
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
