//-------------------------------------------------------------------------------------------------
// <copyright file="AdomdDataProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Syncfusion.Olap.Data;
using MASAC = Microsoft.AnalysisServices.AdomdClient;
using Syncfusion.Olap.Reports;

namespace Syncfusion.Olap.DataProvider
{
    /// <summary>
    /// Exception class holds for the Data Provider.
    /// </summary>
    [Serializable]
    public class DataProviderException
        : Exception
    {
        #region Class constants
        /// <summary>
        /// Default message to show when exception fired.
        /// </summary>
        private const string c_message = @"This operation is invalid in current context.";
        #endregion

        #region Class initialize/finalize methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataProviderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DataProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderException"/> class.
        /// </summary>
        public DataProviderException()
            : this(c_message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public DataProviderException(Exception innerException)
            : this(c_message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        public DataProviderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }

    /// <summary>
    /// Multidimensional data provider that uses ADOMD for .NET to connect and retrieve data from 
    /// the multidimensional data source. 
    /// </summary>
    [Serializable]
    public class AdomdDataProvider : IDataProvider, IDisposable
    {
        [NonSerialized]
        CellSet cellSet;
        [NonSerialized]
        MASAC.AdomdConnection adomdConnectionObjMain = null;
        [NonSerialized]
        MASAC.CellSet adomdCellset;

        string datasource = string.Empty;

        /// <summary>
        /// Gets/sets the connection string for the data adapter;
        /// </summary>
        /// <value></value>
        public string ConnectionString
        {
            get
            {
                return datasource;
            }

            set
            {
                if (datasource != value)
                {
                    datasource = value;
                }
            }
        }

        internal string CurrentCubeName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is property appended.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is property appended; otherwise, <c>false</c>.
        /// </value>
        public bool IsPropertyAppended { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is non SSAS data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is non SSAS data; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Please use ProviderName property.")]
        public bool IsNonSSASData { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public Providers ProviderName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdomdDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AdomdDataProvider(string connectionString)
        {
            datasource = connectionString;
        }

        LevelTypeEnum GetLevelType(MASAC.LevelTypeEnum adomdLevelTypeEnum)
        {
            try
            {
                string strEnum = adomdLevelTypeEnum.ToString();
                LevelTypeEnum levelTypeEnum = (LevelTypeEnum)Enum.Parse(typeof(LevelTypeEnum), strEnum, true);
                double val;
                if (double.TryParse(Convert.ToString(levelTypeEnum), out val))
                    return LevelTypeEnum.Regular;
                else
                    return levelTypeEnum;
            }
            catch
            {
            }

            return LevelTypeEnum.Regular;
        }

        void CheckConnectionState()
        {
            if ((this.adomdConnectionObjMain == null) || (this.adomdConnectionObjMain.State == System.Data.ConnectionState.Broken) || (this.adomdConnectionObjMain.State == System.Data.ConnectionState.Closed))
            {
                this.Connect();
            }
        }

        void Connect()
        {
            try
            {
                if (this.adomdConnectionObjMain == null)
                {
                    this.adomdConnectionObjMain = new MASAC.AdomdConnection(datasource);
                    this.adomdConnectionObjMain.Open();
                }
                else
                {
                    if (this.adomdConnectionObjMain.State != ConnectionState.Open)
                    {
                        this.adomdConnectionObjMain.Open();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataProviderException("Unable to open the connection", ex);
            }
        }

        void Disconnect()
        {
            try
            {
                if (this.adomdConnectionObjMain != null)
                {
                    if (this.adomdConnectionObjMain.State == System.Data.ConnectionState.Open)
                    {
                        this.adomdConnectionObjMain.Close(false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataProviderException("Unable to close the connection", ex);
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void CloseConnection()
        {
            this.Disconnect();
        }

        /// <summary>
        /// Validates the connection string.
        /// </summary>
        /// <returns>
        /// true if connection string has proper syntax
        /// </returns>
        public bool ValidateConnectionString()
        {
            try
            {
                this.Connect();
                if (this.adomdConnectionObjMain.Database != string.Empty)
                {
                    try
                    {
                        this.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        this.Disconnect();
                        throw new DataProviderException("An error occurred while trying to validate the connection", ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Disconnect();
                throw ex;
            }
        }

        #region IDataProvider Members

        /// <summary>
        /// Gets the Cubes in the Specified data source
        /// </summary>
        /// <value>an object of CubeInfoCollection</value>
        public CubeInfoCollection GetCubes
        {
            get
            {
                this.CheckConnectionState();
                CubeInfoCollection cubeInfoCollection = new CubeInfoCollection();
                foreach (MASAC.CubeDef def in adomdConnectionObjMain.Cubes)
                {
                    if (def.Type == Microsoft.AnalysisServices.AdomdClient.CubeType.Cube)
                    {
                        cubeInfoCollection.Add(new CubeInfo(def.Name, def.Caption, def.Description));
                    }
                }

                return cubeInfoCollection;
            }
        }

        /// <summary>
        /// Gets the Cubes in the Specified data source
        /// </summary>
        /// <value>an object of CubeInfoCollection</value>
        /// <remarks>
        /// Returns all cube which checking for the type is cube.
        /// </remarks>
        public CubeInfoCollection GetAllCubes
        {
            get
            {
                this.CheckConnectionState();
                CubeInfoCollection cubeInfoCollection = new CubeInfoCollection();
                foreach (MASAC.CubeDef def in adomdConnectionObjMain.Cubes)
                {
                    cubeInfoCollection.Add(new CubeInfo(def.Name, def.Caption, def.Description));
                }

                return cubeInfoCollection;
            }
        }

        /// <summary>
        /// Gets the measures dimension unique name.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>
        /// measures dimension unique name of type string
        /// </returns>
        public string GetMeasuresDimensionUniqueName(string cubeName)
        {
            MASAC.CubeDef cubeDef = GetCubeDef(cubeName);
            if (cubeDef != null)
            {
                foreach (MASAC.Dimension dimensionCurrent in cubeDef.Dimensions)
                {
                    if (dimensionCurrent.DimensionType == Microsoft.AnalysisServices.AdomdClient.DimensionTypeEnum.Measure)
                    {
                        return dimensionCurrent.UniqueName;
                    }
                }
            }

            return string.Empty;
        }

        MASAC.CubeDef GetCubeDef(string cubeName)
        {
            this.CheckConnectionState();
            MASAC.CubeDef cubeDef = adomdConnectionObjMain.Cubes[cubeName];
            if (cubeDef != null)
            {
                return cubeDef;
            }
            else
            {
                throw new DataException(string.Format("{0} Object not found", cubeName));
            }
        }

        /// <summary>
        /// Get the Member unique name based on the hierarchy
        /// </summary>
        /// <param name="cubeName">cube name.</param>
        /// <param name="hierarchyUniqueName">hierarchy unique name.</param>
        /// <returns>all member unique name</returns>
        public string GetAllMemberUniqueName(string cubeName, string hierarchyUniqueName)
        {
            MASAC.CubeDef cubeDef = GetCubeDef(cubeName);
            if (cubeDef != null)
            {
                MASAC.Hierarchy hierarchyObj = (MASAC.Hierarchy)cubeDef.GetSchemaObject(MASAC.SchemaObjectType.ObjectTypeHierarchy, hierarchyUniqueName);
                MASAC.Property property = hierarchyObj.Properties["ALL_MEMBER"];
                if (property != null)
                {
                    if (property.Value != null)
                    {
                        return property.Value.ToString();
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Executes the count.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <returns>An array of integer.</returns>
        public virtual int[] ExecuteCount(string mdxQuery)
        {
            if (mdxQuery != string.Empty)
            {
                this.CheckConnectionState();
                MASAC.AdomdCommand adomdCommand = new MASAC.AdomdCommand(mdxQuery, adomdConnectionObjMain);
                this.adomdCellset = adomdCommand.ExecuteCellSet();

                if (this.adomdCellset.Axes.Count == 0) return null;
                var counts = new List<int>(2) { 0, 0 };
                var tuples = this.adomdCellset.Axes[0].Set.Tuples;
                for (int i = 0, j = 0; i < this.adomdCellset.Cells.Count && j < tuples.Count; i++, j++)
                {
                    string memberName = tuples[j].Members[0].Name;
                    if (memberName == Syncfusion.Olap.MDXQueryBuilder.QueryBuilderEngine.QueryBuilderEngineVersion3.ColumnsCount)
                        counts[0] = (int)this.adomdCellset.Cells[i].Value;
                    else
                        counts[1] = (int)this.adomdCellset.Cells[i].Value;
                }
                return counts.ToArray();
            }
            return null;
        }

        private bool _isQueryContainsMemberType;

        /// <summary>
        /// Gets or sets the value to display the Localized member property in the output
        /// </summary>
        internal bool ShowLocalizedMemberProperties
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the Command text and returns a cell set
        /// </summary>
        /// <param name="mdxQuery">mdxQuery string passed</param>
        /// <param name="IsGrandTotalOn">if set to <c>true</c> [is non empty cells only].</param>
        /// <param name="isQuery">if set to <c>true</c> [is query].</param>
        /// <param name="currentReport">The current report.</param>
        /// <returns>an object of CellSet</returns>
        public virtual CellSet ExecuteCellSet(string mdxQuery, bool IsGrandTotalOn, bool isQuery, Syncfusion.Olap.Reports.OlapReport currentReport)
        {
            try
            {
                this.IsPropertyAppended = isQuery;
                if (this.IsPropertyAppended)
                {
                    if (this.ProviderName == Providers.Mondrian)
                        mdxQuery = mdxQuery.ToUpper();

                    if (mdxQuery.Contains("member_type") || mdxQuery.Contains("MEMBER_TYPE"))
                    {
                        _isQueryContainsMemberType = true;
                    }
                    else if (_isQueryContainsMemberType)
                    {
                        _isQueryContainsMemberType = false;
                    }
                }
                    
                if (mdxQuery != null && mdxQuery != string.Empty)
                {
                    this.CheckConnectionState();
#if DEBUG
                    var watch = new Stopwatch();
                    watch.Start();
#endif
                    MASAC.AdomdCommand adomdCommand = new MASAC.AdomdCommand(mdxQuery, adomdConnectionObjMain);
                    this.adomdCellset = adomdCommand.ExecuteCellSet();
                    adomdCommand.Dispose();
#if DEBUG
                    watch.Stop();
                    Debug.WriteLine("Actual CellSet Creation time : "+ watch.Elapsed.ToString());
#endif

#if DEBUG
                    watch = new Stopwatch();
                    watch.Start();
#endif
                    cellSet = new CellSet(this);

                    foreach (MASAC.Axis adomdAxis in this.adomdCellset.Axes)
                    {
                        UpdateAxis(adomdAxis, IsGrandTotalOn, currentReport);
                    }

#if DEBUG
                    watch.Stop();
                    Debug.WriteLine("CellSet Creation time : "+ watch.Elapsed.ToString());
#endif
                    return cellSet;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new DataProviderException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="olapReport">The report.</param>
        /// <returns>An object.</returns>
        public virtual object Execute(string commandText, Reports.OlapReport olapReport)
        {
            this.IsPropertyAppended = true;

            if (commandText != null && commandText != string.Empty)
            {
                this.CheckConnectionState();
                MASAC.AdomdCommand adomdCommand = new MASAC.AdomdCommand(commandText, adomdConnectionObjMain);
                var result = adomdCommand.Execute();
                if (result == null) return null;
                return YieldEnumerableResult(result as Microsoft.AnalysisServices.AdomdClient.AdomdDataReader);
            }
            return null;
        }

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="returnResult">if set to <c>true</c> [return result].</param>
        /// <returns>An object.</returns>
        public virtual object Execute(string commandText, bool returnResult)
        {
            this.IsPropertyAppended = true;
            if (commandText != null && commandText != string.Empty)
            {
                this.CheckConnectionState();
                MASAC.AdomdCommand adomdCommand = new MASAC.AdomdCommand(commandText, adomdConnectionObjMain);
                var result = adomdCommand.Execute();
                return returnResult ? result : null;
            }
            return null;
        }

        private object YieldEnumerableResult(MASAC.AdomdDataReader adomdDataReader)
        {
            if (adomdDataReader == null) return null;
            // Expando Collection Implementation
            List<string> headers = new List<string>();
#if SyncfusionFramework4_0
            System.Collections.ObjectModel.ObservableCollection<dynamic> resultSet = new System.Collections.ObjectModel.ObservableCollection<dynamic>();
#else 
            System.Collections.ObjectModel.ObservableCollection<object> resultSet = new System.Collections.ObjectModel.ObservableCollection<object>();
#endif
            var schemaTable = adomdDataReader.GetSchemaTable();

            foreach (System.Data.DataRow schemaRow in schemaTable.Rows)
            {
                headers.Add(schemaRow[0].ToString());
            }

            while (adomdDataReader.Read())
            {
#if SyncfusionFramework4_0
                dynamic expando = new System.Dynamic.ExpandoObject();
                var rowObject = expando as IDictionary<String, object>;
#else
                var rowObject = new Dictionary<string, object>();
#endif
                for (int i = 0; i < adomdDataReader.FieldCount; i++)
                {
                    rowObject[headers[i]] = adomdDataReader.GetValue(i);
                }

                resultSet.Add(rowObject);
            }

            adomdDataReader.Close();

            return resultSet;

            //var schemaTable = adomdDataReader.GetSchemaTable();
            //var typeBuilder = Syncfusion.Olap.Common.RuntimClassGenerator.CreateObject(schemaTable.Rows);
            //System.Collections.ObjectModel.ObservableCollection<object> resultSet = new System.Collections.ObjectModel.ObservableCollection<object>();

            //while (adomdDataReader.Read())
            //{
            //    // Generate our type 
            //    Type generetedType = typeBuilder.CreateType();

            //    // Loop over all the generated properties, and assign the values from our XML: 
            //    System.Reflection.PropertyInfo[] properties = generetedType.GetProperties();

            //    object generatedObject = Activator.CreateInstance(generetedType);

            //    for (int i = 0; i < adomdDataReader.FieldCount; i++)
            //    {
            //        var propertyValue = adomdDataReader.GetValue(i);
            //        properties[i].SetValue(generatedObject, propertyValue, null);
            //    }

            //    resultSet.Add(generetedType);
            //}

            //adomdDataReader.Close();
            //return resultSet.AsEnumerable<object>();
        }

        private void UpdateAxis(MASAC.Axis adomdCurrentAxis, bool IsGrandTotalOn, Syncfusion.Olap.Reports.OlapReport currentReport)
        {
            Axis axis = new Axis();
            axis.Name = adomdCurrentAxis.Name;
            this.cellSet.Axes.Add(axis);
            foreach (MASAC.Tuple adomdCurrentTuple in adomdCurrentAxis.Set.Tuples)
            {
                UpdateTuples(axis, adomdCurrentTuple, IsGrandTotalOn, currentReport);
            }
        }

        private void UpdateTuples(Axis axis, MASAC.Tuple adomdCurrentTuple, bool IsGrandTotalOn, Syncfusion.Olap.Reports.OlapReport currentReport)
        {
            Syncfusion.Olap.Data.Tuple tuple = new Syncfusion.Olap.Data.Tuple();
            axis.TupleSet.Add(tuple);
            int memberCount = 0;
            foreach (MASAC.Member admomdMember in adomdCurrentTuple.Members)
            {
                if (memberCount >= axis.TupleSet.MaxLevel.Count)
                {
                    axis.TupleSet.MaxLevel.Add(0);
                    axis.TupleSet.MinLevel.Add(0);
                    axis.TupleSet.valueSet.Add(false);
                }

                if (admomdMember.LevelDepth > axis.TupleSet.MaxLevel[memberCount])
                {
                    axis.TupleSet.MaxLevel[memberCount] = admomdMember.LevelDepth;
                }

                if (!axis.TupleSet.valueSet[memberCount])
                {
                    axis.TupleSet.MinLevel[memberCount] = admomdMember.LevelDepth;
                    axis.TupleSet.valueSet[memberCount] = true;
                }
                else if (admomdMember.LevelDepth < axis.TupleSet.MinLevel[memberCount])
                {
                    axis.TupleSet.MinLevel[memberCount] = admomdMember.LevelDepth;
                }

                UpdateMembers(axis, tuple, admomdMember, IsGrandTotalOn, currentReport);
                memberCount++;
            }
        }

        private void UpdateParentMember(TupleCollection tupleCollection, string parentUniqueName, Syncfusion.Olap.Data.Member currentMember)
        {
            currentMember.ParentCaption = Convert.ToString(from tuple in tupleCollection
                                                           from member in tuple.Members
                                                           where member.UniqueName == parentUniqueName
                                                           select member.Caption);

        }

        private void UpdateMembers(Axis axis, Syncfusion.Olap.Data.Tuple tuple, MASAC.Member adomdCurrentMember, bool IsGrandTotalOn, Syncfusion.Olap.Reports.OlapReport currentReport)
        {
            Member memberObj = GetMemberFromAdomdMember(adomdCurrentMember, null, null, true, IsGrandTotalOn);
            tuple.Members.Add(memberObj);
            UpdateParentMember(axis.TupleSet, memberObj.ParentUniqueName, memberObj);
            if (currentReport != null)
            {
                InsertMemberProperties(axis, memberObj, adomdCurrentMember, currentReport);
            }
        }

        private void InsertMemberProperties(Axis axis, Syncfusion.Olap.Data.Member memberObj, MASAC.Member adomdCurrentMember, Syncfusion.Olap.Reports.OlapReport currentReport)
        {
            Items items = null;
            if (axis.Name.ToString().ToLower() == "axis0")
                items = currentReport.CategoricalElements;
            else if (axis.Name.ToString().ToLower() == "axis1")
                items = currentReport.SeriesElements;
            if (items != null)
            {
                Item item = items.List.FirstOrDefault(j => j.ElementValue is DimensionElement && (j.ElementValue as DimensionElement).UniqueName.ToUpper() == memberObj.LevelUniqueName.Split('.')[0].ToUpper() && memberObj.LevelUniqueName.ToUpper().Contains("["+(j.ElementValue as DimensionElement).Hierarchy.Name.ToUpper()+"]"));
                if (item != null)
                {
                    DimensionElement dimensionElement = item.ElementValue as Syncfusion.Olap.Reports.DimensionElement;
                    memberObj.ParentHierarchy = dimensionElement.HierarchyName;
                    memberObj.ParentDimension = dimensionElement.Name;
                    CubeSchema cb = null;
                    if (ShowLocalizedMemberProperties)
                    {
                        cb = GetCubeSchema(currentReport.CurrentCubeName); 
                    }
                    foreach (Syncfusion.Olap.Reports.MemberProperty property in dimensionElement.MemberProperties)
                    {
                        MASAC.MemberProperty adomdMemberProperty = adomdCurrentMember.MemberProperties[property.Name];
                        if (adomdMemberProperty != null)
                        {
                            if (ShowLocalizedMemberProperties == true && cb != null)
                            {                                
                                string s = "[" + memberObj.ParentDimension + "].[" + property.Name + "]";
                                Hierarchy hier = cb.GetHierarchyByUniqueName(s);
                                Property memberProperty;
                                if (hier != null) 
                                    memberProperty = new Property(hier.Caption, adomdMemberProperty.Value);
                                else 
                                    memberProperty = new Property(adomdMemberProperty.Name, adomdMemberProperty.Value);
                                memberObj.MemberProperties.Add(memberProperty);
                            }
                            else
                            {
                                Property memberProperty = new Property(adomdMemberProperty.Name, adomdMemberProperty.Value);
                                memberObj.MemberProperties.Add(memberProperty);
                            }
                        }
                    }
                }
            }
        }

        private Syncfusion.Olap.Reports.Items GetAxisItems(Axis axis, Syncfusion.Olap.Reports.OlapReport currentReport)
        {
            if (axis.Name.ToLower() == AxisNum.Axis0.ToString().ToLower())
            {
                return currentReport.CategoricalElements;
            }
            else if (axis.Name.ToLower() == AxisNum.Axis1.ToString().ToLower())
            {
                return currentReport.SeriesElements;
            }
            return null;
        }

        Member GetMemberParentFormAxis(TupleCollection tupleCollection, string memberUniqueName)
        {
            foreach (Syncfusion.Olap.Data.Tuple tuple in tupleCollection)
            {
                foreach (Member member in tuple.Members)
                {
                    if (member.UniqueName == memberUniqueName)
                    {
                        return member;
                    }
                }
            }

            return null;
        }

        Member GetMemberFromAdomdMember(MASAC.Member adomdMemberObj, Level parentLevel, Member parentMember, bool IsDrilledDown, bool IsGrandTotalOn)
        {
            return this.GetMemberFromAdomdMember(adomdMemberObj, parentLevel, parentMember, IsDrilledDown, IsGrandTotalOn, false);
        }

        Member GetMemberFromAdomdMember(MASAC.Member adomdMemberObj, Level parentLevel, Member parentMember, bool IsDrilledDown, bool IsGrandTotalOn, bool memberFromMDX)
        {
            Member member;
            try
            {
                if (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot)
                {
                    member = new Member() { Caption = adomdMemberObj.Caption, UniqueName = adomdMemberObj.UniqueName, Description = adomdMemberObj.Description, LevelDepth = adomdMemberObj.LevelDepth, LevelUniqueName = adomdMemberObj.LevelName, HasChildMembers = (adomdMemberObj.ChildCount > 0 && !IsGrandTotalOn), ParentLevel = parentLevel, _ParentMember = parentMember };
                }
                else
                {
                    member = new Member() { Caption = adomdMemberObj.Caption, UniqueName = adomdMemberObj.UniqueName, Description = adomdMemberObj.Description, LevelDepth = adomdMemberObj.LevelDepth, LevelUniqueName = adomdMemberObj.LevelName, HasChildMembers = (adomdMemberObj.ChildCount > 0 && !IsGrandTotalOn), ParentLevel = parentLevel, _ParentMember = parentMember };
                    if (memberFromMDX)
                        member.Name = (adomdMemberObj.Name == adomdMemberObj.UniqueName) ? Convert.ToString(adomdMemberObj.MemberProperties["MEMBER_NAME"].Value) : adomdMemberObj.Name;
                    else
                        member.Name = adomdMemberObj.Name;
                }
                if (!member.UniqueName.Contains("[Measures]"))
                    member.ParentHierarchy = member.LevelUniqueName.Split('.').Length < 3 ? member.LevelUniqueName.Split('.')[0].Replace("[", "").Replace("]", "") : member.LevelUniqueName.Split('.')[1].Replace("[", "").Replace("]", "");
                if (parentLevel != null)
                {
                    member.CubeSchema = parentLevel.CubeSchema;
                }
                else if (parentMember != null)
                {
                    member.CubeSchema = parentMember.CubeSchema;
                }

                if (this.IsPropertyAppended)
                {
                    try
                    {
#if DEBUG
                        var watch = new Stopwatch();
                        watch.Start();
#endif
                        if (!_isQueryContainsMemberType)
                        {
                            adomdMemberObj.FetchAllProperties();
                        }
#if DEBUG
                        watch.Stop();
                        Debug.WriteLine("Elapsed time for FetchAllProperties method: " + watch.Elapsed.ToString());
#endif
                        member.Type = (MemberTypeEnum)adomdMemberObj.Type;
                    }
                    catch (Exception ex)
                    {
                        //// If it's calculated member then it will throw an exception, since properties will not be available in 
                        //// cube to fetch the result. As a workaround catch the exception and handle it.
                        //// TODO: identify a better approach and fix it
                        if (adomdMemberObj.ParentLevel != null)
                        {
                            if (adomdMemberObj.ParentLevel.LevelType == Microsoft.AnalysisServices.AdomdClient.LevelTypeEnum.Regular)
                            {
                                if (adomdMemberObj.ParentLevel.Caption == "MeasuresLevel")
                                {
                                    member.Type = MemberTypeEnum.Measure;
                                }
                                else
                                {
                                    member.Type = (MemberTypeEnum)adomdMemberObj.ParentLevel.LevelType;
                                }
                            }
                            else
                            {
                                member.Type = (MemberTypeEnum)adomdMemberObj.ParentLevel.LevelType;
                            }
                        }
                        else if (ex.Message.Contains("[Measures]"))
                        {
                            member.Type = MemberTypeEnum.Measure;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    try
                    {
                        member.Type = (MemberTypeEnum)adomdMemberObj.Type;
                    }
                    catch (Exception ex)
                    {
                        if (this.ProviderName == Providers.ActivePivot)
                        {
                            if (adomdMemberObj.LevelName.ToUpper().StartsWith("[" + PropertyConstants.Measures + "]"))
                                member.Type = MemberTypeEnum.Measure;
                            else if (adomdMemberObj.LevelName.Contains("All") || adomdMemberObj.LevelName.Contains("ALL"))
                                member.Type = MemberTypeEnum.All;
                            else if (ex.Message.Contains("[Measures]"))
                                member.Type = MemberTypeEnum.Measure;
                            else
                                member.Type = MemberTypeEnum.Regular;
                        }
                        else if (this.ProviderName == Providers.Mondrian)
                        {
                            if (adomdMemberObj.ParentLevel != null)
                            {
                                if (adomdMemberObj.ParentLevel.Caption == "MeasuresLevel")
                                    member.Type = MemberTypeEnum.Measure;
                                //else if (adomdMemberObj.ParentLevel.LevelType.ToString() == "All" || adomdMemberObj.LevelDepth == 0)
                                else if (adomdMemberObj.ParentLevel.LevelType.ToString() == "All")
                                    member.Type = MemberTypeEnum.All;
                            }
                            else if (ex.Message.Contains("[Measures]"))
                            {
                                member.Type = MemberTypeEnum.Measure;
                            }
                            else
                                member.Type = MemberTypeEnum.Regular;
                        }
                        else if (this.ProviderName == Providers.SSAS)
                        {
                            if (adomdMemberObj.ParentLevel != null)
                            {
                                if (adomdMemberObj.ParentLevel.LevelType == Microsoft.AnalysisServices.AdomdClient.LevelTypeEnum.Regular)
                                {
                                    if (adomdMemberObj.ParentLevel.Caption == "MeasuresLevel")
                                    {
                                        member.Type = MemberTypeEnum.Measure;
                                    }
                                    else
                                    {
                                        member.Type = (MemberTypeEnum)adomdMemberObj.ParentLevel.LevelType;
                                    }
                                }
                                else
                                {
                                    member.Type = (MemberTypeEnum)adomdMemberObj.ParentLevel.LevelType;
                                }
                            }
                            else if (ex.Message.Contains("[Measures]"))
                            {
                                member.Type = MemberTypeEnum.Measure;
                            }
                        }
                        else
                        {
                            throw ex;
                        }
                    }

                    if (adomdMemberObj.MemberProperties.Count > 0)
                    {
                        string property = "";
                        try
                        {
                            property = (string)adomdMemberObj.MemberProperties.Find("PARENT_UNIQUE_NAME").Value;
                        }
                        catch (Exception ex)
                        {

                        }
                        string[] names = member.UniqueName.Split('.');
                        if (property != null)
                        {
                            if (property.StartsWith(names[0]))
                                member.ParentUniqueName = property;
                            else
                                member.ParentUniqueName = names[0] + "." + property;
                        }
                    }
                }

                if (IsDrilledDown)
                {
                    member.DrilledDown = adomdMemberObj.DrilledDown;
                }

                if (member.Name.Length == 0)
                {
                    member.Visible = false;
                }

                if (member.Caption.Length == 0)
                {
                    member.Caption = "(Blank)";
                }
                member.Properties.Add(PropertyConstants.MemberName, adomdMemberObj);
                return member;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>A collection of child members</returns>
        public MemberCollection GetChildMembers(Member member, bool IsGrandTotalOn)
        {
            this.CheckConnectionState();
            MemberCollection memberCollection = new MemberCollection(member);
            MASAC.Member adomdParentMember = null;
            Property property = member.Properties.FindByName(PropertyConstants.MemberName);
            if (property != null)
            {
                adomdParentMember = property.Value as MASAC.Member;
                if (adomdParentMember != null)
                {
                    memberCollection = this.GetChildren(null, member, 0, 0, IsGrandTotalOn);
                }
            }

            return memberCollection;
        }
        
        public MemberCollection GetChildrenByMDX(string commandText)
        {
            this.CheckConnectionState();
            MemberCollection memberCollection = new MemberCollection();
            MASAC.CellSet set = new MASAC.AdomdCommand(commandText, this.adomdConnectionObjMain).ExecuteCellSet();
            MASAC.Axis axis = set.Axes[0];
            string substring = commandText.Substring(commandText.IndexOf('['));
            string parentUniqName = substring.Remove(substring.IndexOf(".CHILDREN"));
            for (int i = 0; i < axis.Positions.Count; i++)
            {
                memberCollection.Add(this.GetMemberFromAdomdMember(axis.Positions[i].Members[0], null, null, false, false, true));
                memberCollection[i].ParentUniqueName = parentUniqName;
            }
            return memberCollection;
        }

        private MemberCollection GetChildren(MASAC.Level level, Member parentMember, int memberAxisPosition, int memberHierarhcyPosition, bool IsGrandTotalOn)
        {
            this.CheckConnectionState();
            MemberCollection memberCollection = new MemberCollection();
            string memberMdxQuery = "SELECT ADDCALCULATEDMEMBERS({" + parentMember.UniqueName + ".CHILDREN}) DIMENSION PROPERTIES MEMBER_NAME, MEMBER_TYPE ON 0, {} ON 1 FROM [" + this.CurrentCubeName + "]";
            MASAC.CellSet set = new MASAC.AdomdCommand(memberMdxQuery, this.adomdConnectionObjMain).ExecuteCellSet();
            MASAC.Axis axis = set.Axes[memberAxisPosition];
            for (int i = 0; i < axis.Positions.Count; i++)
            {
                memberCollection.Add(this.GetMemberFromAdomdMember(axis.Positions[i].Members[0], null, parentMember, false, IsGrandTotalOn, true));
            }
            return memberCollection;
        }

        public MemberCollection GetMembersUsingMDX(string memberUniqueName, int memberAxisPosition, int memberHierarhcyPosition, bool IsGrandTotalOn, NodeTypes memberType)
        {
            return this.GetMembersUsingMDX(memberUniqueName, memberAxisPosition, memberHierarhcyPosition, IsGrandTotalOn, memberType, this.CurrentCubeName);
        }

        public MemberCollection GetMembersUsingMDX(string memberUniqueName, int memberAxisPosition, int memberHierarhcyPosition, bool IsGrandTotalOn, NodeTypes memberType, string cubeName)
        {
            this.CheckConnectionState();
            MemberCollection memberCollection = new MemberCollection();
            string memberMdxQuery = string.Empty;
            if (memberType == NodeTypes.Parent)
                memberMdxQuery = "SELECT ADDCALCULATEDMEMBERS({" + memberUniqueName + "}) DIMENSION PROPERTIES MEMBER_NAME, MEMBER_TYPE ON 0, {} ON 1 FROM [" + cubeName + "]";
            else
                memberMdxQuery = "SELECT ADDCALCULATEDMEMBERS({" + memberUniqueName + ".CHILDREN}) DIMENSION PROPERTIES MEMBER_NAME, MEMBER_TYPE ON 0, {} ON 1 FROM [" + cubeName + "]";

            MASAC.CellSet set = new MASAC.AdomdCommand(memberMdxQuery, this.adomdConnectionObjMain).ExecuteCellSet();
            MASAC.Axis axis = set.Axes[memberAxisPosition];
            for (int i = 0; i < axis.Positions.Count; i++)
            {
                memberCollection.Add(this.GetMemberFromAdomdMember(axis.Positions[i].Members[0], null, null, false, IsGrandTotalOn, true));
            }
            return memberCollection;
        }

        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <param name="memberUniqueName">Unique name of the member.</param>
        /// <param name="cubeName">Name of the cube.</param>
        /// <param name="IsGrandTotalOn">if set to <c>true</c> [is grand total on].</param>
        /// <returns>Member Collection</returns>
        public MemberCollection GetChildMembers(string memberUniqueName, string cubeName, bool IsGrandTotalOn)
        {
            this.CheckConnectionState();
            MASAC.Level adomdlevelObj = null;
            if (!string.IsNullOrEmpty(memberUniqueName))
            {
                MemberCollection memberCollection = new MemberCollection();
                List<string> elementNames = new System.Collections.Generic.List<string>();
                if (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot)
                {
                    System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"\[(.*?)\]");
                    string[] uniqueNames = regEx.Split(memberUniqueName);
                    foreach (var item in uniqueNames)
                    {
                        if (!string.IsNullOrEmpty(item) && !item.Equals("."))
                        {
                            elementNames.Add(GetUniqueName(item));
                        }
                    }
                }
                else
                {
                    elementNames = memberUniqueName.Split('.').ToList();
                }

                if (elementNames.Count() >= 3 || (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot))
                {
                    //CubeDef cubeDef = this.adomdConnectionObjMain.Cubes.Find(cubeName);
                    MASAC.CubeDef cubeDef = null;
                    for (int i = 0; i < this.adomdConnectionObjMain.Cubes.Count; i++)
                    {
                        if (this.adomdConnectionObjMain.Cubes[i].Name.ToUpper() == cubeName.ToUpper())
                        {
                            cubeDef = this.adomdConnectionObjMain.Cubes[i];
                            break;
                        }
                    }
                    if (cubeDef != null && !string.IsNullOrEmpty(elementNames[0]) && !string.IsNullOrEmpty(elementNames[1]))
                    {
                        //MASAC.Dimension adomdDimension = cubeDef.Dimensions.Find(GetSimpleName(elementNames[0]));
                        MASAC.Dimension adomdDimension = null;
                        for (int i = 0; i < cubeDef.Dimensions.Count; i++)
                        {
                            if (cubeDef.Dimensions[i].Name == GetSimpleName(elementNames[0]))
                            {
                                adomdDimension = cubeDef.Dimensions[i];
                                break;
                            }
                        }
                        if (adomdDimension == null && (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot) && elementNames[0].Contains('.'))
                        {
                            string[] tempNames = elementNames[0].Split('.');
                            //adomdDimension = cubeDef.Dimensions.Find(GetSimpleName(tempNames[0]));
                            for (int i = 0; i < cubeDef.Dimensions.Count; i++)
                            {
                                if (cubeDef.Dimensions[i].Name == GetSimpleName(tempNames[0]))
                                {
                                    adomdDimension = cubeDef.Dimensions[i];
                                    break;
                                }
                            }
                        }

                        if (adomdDimension != null)
                        {
                            MASAC.Hierarchy adomdHierarcy = null;
                            if (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot)
                            {
                                foreach (var item in adomdDimension.Hierarchies)
                                {
                                    if (item.UniqueName.Contains(elementNames[0]))
                                    {
                                        adomdHierarcy = (MASAC.Hierarchy)item;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                //adomdHierarcy = adomdDimension.Hierarchies.Find(GetSimpleName(elementNames[1]));
                                for (int i = 0; i < adomdDimension.Hierarchies.Count; i++)
                                {
                                    if (adomdDimension.Hierarchies[i].Name == (GetSimpleName(elementNames[1])))
                                    {
                                        adomdHierarcy = adomdDimension.Hierarchies[i];
                                        break;
                                    }
                                }
                            }

                            if (adomdHierarcy != null)
                            {
                                if (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot)
                                {
                                    //adomdlevelObj = adomdHierarcy.Levels.Find(GetSimpleName(elementNames[1]));
                                    for (int i = 0; i < adomdHierarcy.Levels.Count; i++)
                                    {
                                        if (adomdHierarcy.Levels[i].Name == GetSimpleName(elementNames[1]))
                                        {
                                            adomdlevelObj = adomdHierarcy.Levels[i];
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                  //  adomdlevelObj = adomdHierarcy.Levels.Find(GetSimpleName(elementNames[2]));
                                    for (int i = 0; i < adomdHierarcy.Levels.Count; i++)
                                    {
                                        if (adomdHierarcy.Levels[i].Name == GetSimpleName(elementNames[1]))
                                        {
                                            adomdlevelObj = adomdHierarcy.Levels[i];
                                            break;
                                        }
                                    }
                                }

                                if (adomdlevelObj != null)
                                {
                                    if (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot)
                                    {
                                        memberUniqueName = elementNames[0];
                                        for (int i = 2; i < elementNames.Count(); i++)
                                        {
                                            memberUniqueName += "." + elementNames[i];
                                        }
                                    }

                                    foreach (MASAC.Member member in adomdlevelObj.GetMembers())
                                    {
                                        if (member.UniqueName.Equals(memberUniqueName, StringComparison.InvariantCultureIgnoreCase) || member.UniqueName.Equals(String.Join(".", elementNames.ToArray()), StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            memberCollection = GetMemberCollection(member, IsGrandTotalOn);
                                            return memberCollection;
                                        }
                                    }
                                }
                                else if (adomdHierarcy.DefaultMember == memberUniqueName)
                                {
                                    MASAC.Member defaultMember = null;
                                    foreach (MASAC.Level lvl in adomdHierarcy.Levels)
                                    {
                                        MASAC.MemberCollection members = lvl.GetMembers(0, 1, new MASAC.MemberFilter("UniqueName", memberUniqueName));
                                        if (members != null)
                                        {
                                            defaultMember = members[0];
                                            break;
                                        }
                                    }
                                    if (defaultMember != null)
                                    {
                                        memberCollection = GetMemberCollection(defaultMember, IsGrandTotalOn);
                                        return memberCollection;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < adomdHierarcy.Levels.Count; i++)
                                    {
                                        MASAC.MemberCollection tempMember = adomdHierarcy.Levels[i].GetMembers();
                                        foreach (MASAC.Member member in tempMember)
                                        {
                                            if (member.UniqueName.Equals(memberUniqueName, StringComparison.InvariantCultureIgnoreCase) || member.UniqueName.Equals(String.Join(".", elementNames.ToArray()), StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                memberCollection = GetMemberCollection(member, IsGrandTotalOn);
                                                return memberCollection;
                                            }
                                        }
                                    }
                                }
                                if (memberCollection.Count == 0)
                                {
                                    for (int i = 0; i < adomdHierarcy.Levels.Count; i++)
                                    {
                                        MASAC.MemberCollection tempMember = adomdHierarcy.Levels[i].GetMembers();
                                        foreach (MASAC.Member member in tempMember)
                                        {
                                            if (member.UniqueName.Equals(memberUniqueName, StringComparison.InvariantCultureIgnoreCase) || member.UniqueName.Equals(String.Join(".", elementNames.ToArray()), StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                memberCollection = GetMemberCollection(member, IsGrandTotalOn);
                                                return memberCollection;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private MemberCollection GetMemberCollection(MASAC.Member member, bool grantTotal)
        {
            MemberCollection memberCollection = null;

            MASAC.MemberCollection adomdMemberCollection = member.ChildCount > 1000 ? member.GetChildren(0, 1000) : member.GetChildren();
            if (adomdMemberCollection != null)
            {
                memberCollection = new MemberCollection();
                foreach (MASAC.Member adomdMember in adomdMemberCollection)
                {
                    memberCollection.Add(this.GetMemberFromAdomdMember(adomdMember, null, null, false, grantTotal));
                }
            }
            return memberCollection;
        }

        /// <summary>
        /// Gets the cube schema.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>an object of CubeSchema</returns>
        public CubeSchema GetCubeSchema(string cubeName)
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            this.CheckConnectionState();
            CubeSchema cubeSchema = new CubeSchema(this);
            if (cubeName == string.Empty || cubeName == null)
            {
                throw new DataProviderException("Invalid CubeName");
            }

            //MASAC.CubeDef current = this.adomdConnectionObjMain.Cubes.Find(cubeName);
            MASAC.CubeDef current = null;
            for (int i = 0; i < this.adomdConnectionObjMain.Cubes.Count; i++)
            {
                if (this.adomdConnectionObjMain.Cubes[i].Name.ToUpper() == cubeName.ToUpper())
                {
                    current = this.adomdConnectionObjMain.Cubes[i];
                    break;
                }
            }
            if (current != null)
            {
                cubeSchema.CubeInfo = new CubeInfo(current.Name, current.Caption, current.Description);

                //// TODO: Named Set Not implemented

                //// Updating the Measure elements in the CubeSchema
                foreach (MASAC.Measure adomdMeasure in current.Measures)
                {
                    UpdateMeasures(cubeSchema, adomdMeasure);
                }

                //// Updating KPI elements in CubeSchema
                try
                {
                    foreach (MASAC.Kpi adomdKpi in current.Kpis)
                    {
                        UpdateKPI(cubeSchema, adomdKpi);
                    }
                }
                catch
                { }

                //// updating the dimension elements in the CubeSchema
                foreach (MASAC.Dimension adomdDimension in current.Dimensions)
                {
                    UpdateDimension(cubeSchema, adomdDimension);
                }

                try
                {
                    //// updating the namedSet elements in the CubeSchema
                    foreach (MASAC.NamedSet adomdNamedSet in current.NamedSets)
                    {
                        UpdateNamedSet(cubeSchema, adomdNamedSet);
                    }
                }
                catch
                { }
            }
#if DEBUG
            sw.Stop();
            Console.WriteLine("Time taken form AdomdProvider.GetCubeSchema {0}", sw.Elapsed);
#endif
            //// Return the CubeSchema Object
            return cubeSchema;
        }

        private void UpdateMeasures(CubeSchema cubeSchema, MASAC.Measure adomdMeasure)
        {
            Measure measureObj = new Measure();
            measureObj.Name = adomdMeasure.Name;
            measureObj.Description = adomdMeasure.Description;
            measureObj.Caption = adomdMeasure.Caption;
            measureObj.UniqueName = adomdMeasure.UniqueName;
            MASAC.Property prop = adomdMeasure.Properties.Find("NUMERIC_PRECISION");
            try
            {
                if (prop != null)
                {
                    measureObj.NumericPrecision = adomdMeasure.NumericPrecision;
                    measureObj.NumericScale = adomdMeasure.NumericScale;
                    measureObj.Units = adomdMeasure.Units;
                }
                else
                {
                    if (this.ProviderName != Providers.ActivePivot)
                        this.ProviderName = Providers.Mondrian;
                }
            }
            catch
            {                
                if (this.ProviderName != Providers.ActivePivot)
                    this.ProviderName = Providers.Mondrian;
            }
            MASAC.Property propertyAggregator = adomdMeasure.Properties.Find(PropertyConstants.Measure_Aggregator);
            measureObj.MeasureAggregator = (int)propertyAggregator.Value;
            MASAC.Property propertyObj = adomdMeasure.Properties.Find(PropertyConstants.AdomdMeasureDisplayFolder);
            if (propertyObj != null)
            {
                if (propertyObj.Value == null || string.IsNullOrEmpty(propertyObj.Value.ToString()))
                {
                    propertyObj = adomdMeasure.Properties.Find(PropertyConstants.AdomdMeasureGroupName);
                    measureObj.GroupName = (propertyObj == null || propertyObj.Value == null) ? string.Empty : (string)propertyObj.Value;
                }
                else
                {
                    measureObj.DisplayFolder = (string)propertyObj.Value;
                    propertyObj = adomdMeasure.Properties.Find(PropertyConstants.AdomdMeasureGroupName);
                    measureObj.GroupName = (propertyObj == null || propertyObj.Value == null) ? string.Empty : (string)propertyObj.Value;
                }
            }

            cubeSchema.Measures.Add(measureObj);
        }

        private void UpdateNamedSet(CubeSchema cubeSchema, MASAC.NamedSet adomdNamedSet)
        {
            NamedSet namedSetObj = new NamedSet();
            namedSetObj.Name = adomdNamedSet.Name;
            try
            {
                namedSetObj.Expression = adomdNamedSet.Expression;
                namedSetObj.Description = adomdNamedSet.Description;
#if !SyncfusionFramework3_5
                namedSetObj.DisplayFolder = adomdNamedSet.DisplayFolder;
#endif
            }
            catch
            {   
                if (this.ProviderName != Providers.ActivePivot)
                    this.ProviderName = Providers.Mondrian;
            }

            namedSetObj.Properties.Add(new Property(PropertyConstants.KPI, adomdNamedSet));
            MASAC.Property property = adomdNamedSet.Properties[PropertyConstants.NamedSetDimension];
            if (property != null && property.Value is string)
            {
                namedSetObj.ParentHierarchyName = (string)property.Value;
                Hierarchy hierarchyObj = cubeSchema.GetHierarchyByUniqueName(namedSetObj.ParentHierarchyName);
                if (hierarchyObj != null)
                {
                    namedSetObj.ParentDimensionName = hierarchyObj.ParentDimension.Name;
                }
            }

            cubeSchema.NamedSets.Add(namedSetObj);
        }

        private void UpdateKPI(CubeSchema cubeSchema, MASAC.Kpi adomdKpi)
        {
            Kpi kpiObj = new Kpi();
            kpiObj.Name = adomdKpi.Name;
            kpiObj.Description = adomdKpi.Description;
            kpiObj.Caption = adomdKpi.Caption;
            kpiObj.DisplayFolder = adomdKpi.DisplayFolder.StartsWith("\\") ? adomdKpi.DisplayFolder.Remove(adomdKpi.DisplayFolder.IndexOf("\\"), 1) : adomdKpi.DisplayFolder;
            kpiObj.StatusGraphic = adomdKpi.StatusGraphic;
            kpiObj.TrendGraphic = adomdKpi.TrendGraphic;
            kpiObj.ParentKpi = adomdKpi.ParentKpi;
            kpiObj.Properties.Add(new Property(PropertyConstants.KPI, adomdKpi));
            cubeSchema.Kpis.Add(kpiObj);
        }

        private void UpdateDimension(CubeSchema cubeSchema, MASAC.Dimension adomdDimension)
        {
            if (adomdDimension.DimensionType != MASAC.DimensionTypeEnum.Measure)
            {
                Dimension dimensionObj = new Dimension();
                dimensionObj.Name = adomdDimension.Name;
                dimensionObj.UniqueName = adomdDimension.UniqueName;
                ////Setting the dimension type
                dimensionObj.DimensionType = (DimensionTypeEnum)Enum.Parse(typeof(DimensionTypeEnum), adomdDimension.DimensionType.ToString());
                dimensionObj.Caption = adomdDimension.Caption;
                dimensionObj.Description = adomdDimension.Description;
                dimensionObj.ParentCubeSchema = cubeSchema;
                cubeSchema.Dimensions.Add(dimensionObj);
                MASAC.Property property = adomdDimension.Properties.Find(PropertyConstants.DefaultHierarchy);
                if (property != null && property.Value is string)
                {
                    if (!(property.Value.ToString().StartsWith("[")) && this.ProviderName == Providers.ActivePivot)
                    {
                        string propertyValue = "[" + (string)property.Value + "]";
                        if (propertyValue.StartsWith(dimensionObj.UniqueName))
                            dimensionObj.DefaultHierarchyName = propertyValue;
                        else
                            dimensionObj.DefaultHierarchyName = dimensionObj.UniqueName + "." + propertyValue;
                    }
                    else
                    {
                        if (property.Value.ToString().StartsWith(dimensionObj.UniqueName))
                            dimensionObj.DefaultHierarchyName = (string)property.Value;
                        else
                            dimensionObj.DefaultHierarchyName = dimensionObj.UniqueName + "." + (string)property.Value;
                    }
                }

                //// Adding the hierarchies to the dimension
                foreach (MASAC.Hierarchy adomdHierarchy in adomdDimension.Hierarchies)
                {
                    UpdateHierarchy(cubeSchema, dimensionObj, adomdHierarchy);
                }
            }
        }

        private void UpdateHierarchy(CubeSchema cubeSchema, Dimension dimensionObj, MASAC.Hierarchy adomdHierarchy)
        {
            bool isAttributeInHierarchy = false;
            if (adomdHierarchy.HierarchyOrigin == MASAC.HierarchyOrigin.AttributeHierarchy)
            {
                isAttributeInHierarchy = true;
            }

            Hierarchy hierarchyObj = new Hierarchy();
            try
            {
                hierarchyObj.DisplayFolder = adomdHierarchy.DisplayFolder;
            }
            catch
            {
                hierarchyObj.DisplayFolder = string.Empty;
                if (this.ProviderName != Providers.ActivePivot)
                    this.ProviderName = Providers.Mondrian;
            }
            hierarchyObj.Name = adomdHierarchy.Name;
            hierarchyObj.Caption = adomdHierarchy.Caption;
            hierarchyObj.Description = adomdHierarchy.Description;
            hierarchyObj.IsAttributeHierarchy = isAttributeInHierarchy;
            hierarchyObj.UniqueName = adomdHierarchy.UniqueName;
            hierarchyObj.ParentDimension = dimensionObj;
            if (!hierarchyObj.UniqueName.StartsWith(hierarchyObj.ParentDimension.UniqueName))
                hierarchyObj.UniqueName = hierarchyObj.ParentDimension.UniqueName + "." + adomdHierarchy.UniqueName;
            hierarchyObj.DefaultMemberUniqueName = adomdHierarchy.DefaultMember;
            dimensionObj.Hierarchies.Add(hierarchyObj);
            bool isFirstLevel = true;
            foreach (MASAC.Level adomdLevel in adomdHierarchy.Levels)
            {
                Level level = GetLevelFormAdomdLevel(cubeSchema, hierarchyObj, adomdLevel);
                if ((level.LevelType != LevelTypeEnum.All || this.ProviderName == Providers.ActivePivot) && isFirstLevel)
                {
                    hierarchyObj.DefaultLevelName = level.Name;
                    hierarchyObj.DefaultLevelUniqueName = level.UniqueName;
                    isFirstLevel = false;
                }

                hierarchyObj.Levels.Add(level);
            }
        }

        private Level GetLevelFormAdomdLevel(CubeSchema cubeSchema, Hierarchy hierarchyObj, MASAC.Level adomdLevel)
        {
            Level level = new Level();
            level.LevelType = GetLevelType(adomdLevel.LevelType);
            level.CubeSchema = cubeSchema;
            level.Name = adomdLevel.Name;
            level.UniqueName = adomdLevel.UniqueName;
            level.Caption = adomdLevel.Caption;
            level.Description = adomdLevel.Description;
            level.ParentHierarchy = hierarchyObj;

            if (!adomdLevel.UniqueName.StartsWith(level.ParentHierarchy.ParentDimension.UniqueName))
                level.UniqueName = adomdLevel.ParentHierarchy.ParentDimension.UniqueName + "." + adomdLevel.UniqueName;

            level.IsMemberLoadedOnDemand = false;
            return level;
        }

        /// <summary>
        /// Gets the level members.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>an object of MemberCollection</returns>
        public MemberCollection GetLevelMembers(Level level)
        {
            MASAC.Level adomdlevelObj = null;
            this.CheckConnectionState();
            //MASAC.CubeDef cubeDef = this.adomdConnectionObjMain.Cubes.Find(level.CubeSchema.CubeInfo.Name);
            MASAC.CubeDef cubeDef = null;
            for (int i = 0; i < this.adomdConnectionObjMain.Cubes.Count; i++)
            {
                if (this.adomdConnectionObjMain.Cubes[i].Name == level.CubeSchema.CubeInfo.Name)
                {
                    cubeDef = this.adomdConnectionObjMain.Cubes[i];
                    break;
                }
            }
            if (cubeDef != null && level.ParentHierarchy != null && level.ParentHierarchy.ParentDimension != null)
            {
                MASAC.Dimension adomdDimension = null;
                //MASAC.Dimension adomdDimension = cubeDef.Dimensions.Find(level.ParentHierarchy.ParentDimension.Name);
                for (int i = 0; i < cubeDef.Dimensions.Count; i++)
                {
                    if (cubeDef.Dimensions[i].Name == level.ParentHierarchy.ParentDimension.Name)
                    {
                        adomdDimension = cubeDef.Dimensions[i];
                        break;
                    }
                }
                if (adomdDimension != null)
                {
                    MASAC.Hierarchy adomdHierarcy=null;
                   // MASAC.Hierarchy adomdHierarcy = adomdDimension.Hierarchies.Find(level.ParentHierarchy.Name);
                    for (int i = 0; i < adomdDimension.Hierarchies.Count; i++)
                    {
                        if (adomdDimension.Hierarchies[i].Name == level.ParentHierarchy.Name)
                        {
                            adomdHierarcy = adomdDimension.Hierarchies[i];
                            break;
                        }
                    }
                    if (adomdHierarcy != null)
                    {
                        //adomdlevelObj = adomdHierarcy.Levels.Find(level.Name);
                        for (int i = 0; i < adomdHierarcy.Levels.Count; i++)
                        {
                            if (adomdHierarcy.Levels[i].Name == level.Name)
                            {
                                adomdlevelObj = adomdHierarcy.Levels[i];
                                break;
                            }
                        }
                    }
                }
            }

            MemberCollection memberCollection = new MemberCollection(level);
            if (adomdlevelObj != null)
            {
                MASAC.MemberCollection adomdMemberCollection;
                adomdMemberCollection = adomdlevelObj.GetMembers();
                foreach (var adomdMember in adomdMemberCollection)
                {
                    memberCollection.Add(this.GetMemberFromAdomdMember(adomdMember, level, null, false, false));
                }
            }

            return memberCollection;
        }

        /// <summary>
        /// Gets the level members.
        /// </summary>
        /// <param name="levelUniqueName">Unique name of a level level element</param>
        /// <param name="cubeName">Name of the cube</param>
        /// <returns>returns a MemberCollection object</returns>
        public MemberCollection GetLevelMembers(string levelUniqueName, string cubeName)
        {
            MASAC.Level adomdlevelObj = null;
            this.CheckConnectionState();
            List<string> elementNames = new System.Collections.Generic.List<string>();
            //if (this.IsMondrianSupported)
            if (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot)
            {
                System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"\[(.*?)\]");
                string[] uniqueNames = regEx.Split(levelUniqueName);
                foreach (var item in uniqueNames)
                {
                    if (!string.IsNullOrEmpty(item) && !item.Equals("."))
                    {
                        elementNames.Add(GetUniqueName(item));
                    }
                }
            }
            else
            {
                elementNames = levelUniqueName.Split('.').ToList();
            }
            //if (elementNames.Count() >= 3 || (this.IsMondrianSupported && elementNames.Count() >= 2))
            if ((elementNames.Count>=3) || (elementNames.Count() >= 2 && this.ProviderName == Providers.SSAS) || ((this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot) && elementNames.Count() >= 2))
            {
                //MASAC.CubeDef cubeDef = this.adomdConnectionObjMain.Cubes.Find(cubeName);
                MASAC.CubeDef cubeDef = null;
                for (int i = 0; i < this.adomdConnectionObjMain.Cubes.Count; i++)
                {
                    if (this.adomdConnectionObjMain.Cubes[i].Name.ToUpper() == cubeName.ToUpper())
                    {
                        cubeDef = this.adomdConnectionObjMain.Cubes[i];
                        break;
                    }
                }
                if (cubeDef != null && !string.IsNullOrEmpty(elementNames[0]) && !string.IsNullOrEmpty(elementNames[1]))
                {
                    //MASAC.Dimension adomdDimension = cubeDef.Dimensions.Find(GetSimpleName(elementNames[0]));
                    MASAC.Dimension adomdDimension = null;
                    for (int i = 0; i < cubeDef.Dimensions.Count; i++)
                    {
                        if (cubeDef.Dimensions[i].Name == GetSimpleName(elementNames[0]))
                        {
                            adomdDimension = cubeDef.Dimensions[i];
                            break;
                        }
                    }
                    if (adomdDimension != null)
                    {
                        MASAC.Hierarchy adomdHierarcy = null;
                        //if (this.IsMondrianSupported)
                        if (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot)
                        {
                            string hierarchyUniqueName;
                            if (elementNames.Count() > 2)
                            {
                                hierarchyUniqueName = elementNames[1];
                            }
                            else
                            {
                                hierarchyUniqueName = elementNames[0];
                            }
                            foreach (var item in adomdDimension.Hierarchies)
                            {
                                if (item.UniqueName.Equals(hierarchyUniqueName))
                                {
                                    adomdHierarcy = (MASAC.Hierarchy)item;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //adomdHierarcy = adomdDimension.Hierarchies.Find(GetSimpleName(elementNames[1]));
                            for (int i = 0; i < adomdDimension.Hierarchies.Count; i++)
                            {
                                if (adomdDimension.Hierarchies[i].Name == GetSimpleName(elementNames[1]))
                                {
                                    adomdHierarcy = adomdDimension.Hierarchies[i];
                                    break;
                                }
                            }
                        }
                        if (adomdHierarcy != null)
                        {
                            //if (this.IsMondrianSupported)
                            if (this.ProviderName == Providers.Mondrian || this.ProviderName == Providers.ActivePivot)
                            {
                                string levelName;
                                if (elementNames.Count() > 2)
                                {
                                    levelName = elementNames[2];
                                }
                                else
                                {
                                    levelName = elementNames[1];
                                }
                                //foreach (var item in adomdHierarcy.Levels)
                                //{
                                //    if (item.UniqueName.Equals(GetUniqueName(hierarchyUniqueName)))
                                //    {
                                //        adomdlevelObj = (MASAC.Level)item;
                                //        break;
                                //    }
                                //}
                                //adomdlevelObj = adomdHierarcy.Levels.Find(GetSimpleName(levelName))
                                for (int i = 0; i < adomdHierarcy.Levels.Count; i++)
                                {
                                    if (adomdHierarcy.Levels[i].Name == GetSimpleName(levelName))
                                    {
                                        adomdlevelObj = adomdHierarcy.Levels[i];
                                        break;
                                    }
                                }
                            }
                            else if (elementNames.Count() == 2)
                            {
                                adomdlevelObj = adomdHierarcy.Levels.Find(GetSimpleName("[(All)]"));
                            }
                            else
                            {
                                //adomdlevelObj = adomdHierarcy.Levels.Find(GetSimpleName(elementNames[2]));
                                for (int i = 0; i < adomdHierarcy.Levels.Count; i++)
                                {
                                    if (adomdHierarcy.Levels[i].Name == GetSimpleName(elementNames[2]))
                                    {
                                        adomdlevelObj = adomdHierarcy.Levels[i];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                MemberCollection memberCollection = new MemberCollection();
                if (adomdlevelObj != null)
                {
                    MASAC.MemberCollection adomdMemberCollection = adomdlevelObj.MemberCount > 7000 ? adomdlevelObj.GetMembers(0, 7000) : adomdlevelObj.GetMembers();

                    foreach (var adomdMember in adomdMemberCollection)
                    {
                        memberCollection.Add(this.GetMemberFromAdomdMember(adomdMember, null, null, false, false));
                    }
                }

                return memberCollection;
            }
            return null;
        }

        private string GetSimpleName(string nameWithBrase)
        {
            return (nameWithBrase.Replace("[", string.Empty).Replace("]", string.Empty));
        }

        private string GetUniqueName(string nameWithoutBrase)
        {
            return string.Format("[" + nameWithoutBrase + "]");
        }

        /// <summary>
        /// Gets the parent member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>a Member object</returns>
        public Member GetParentMember(Member member)
        {
            this.CheckConnectionState();
            Member memberObj = null;
            MASAC.Member adomdMember = null;
            Property property = member.Properties.FindByName("AdomdMember");
            if (property != null)
            {
                adomdMember = property.Value as MASAC.Member;

                if (adomdMember != null)
                {
                    MASAC.Member adomdParentMember = adomdMember.Parent;
                    if (adomdParentMember != null)
                    {
                        memberObj = this.GetMemberFromAdomdMember(adomdParentMember, null, null, false, false);
                    }
                }
            }

            return memberObj;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="indexes">The indexes.</param>
        /// <returns>a cell object</returns>
        public virtual Cell GetCell(CellSet cellSet, params int[] indexes)
        {
            if (this.adomdCellset.Cells.Count > 0)
            {
                string formatString = string.Empty;
                try
                {
                    MASAC.Cell cell = this.adomdCellset.Cells[indexes];
                    //if (!this.IsPropertyAppended)
                    {
                        var property = cell.CellProperties.Find("FORMAT_STRING");
                        formatString = property == null || property.Value == null ? string.Empty : property.Value.ToString();
                    }
                    return new Cell(cell.Value, cell.FormattedValue, formatString);
                }
                catch (Exception)
                {
                    return new Cell("Error", "Error", formatString);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether [has valid cells].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has valid cells]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidCells()
        {
            if (this.adomdCellset.Cells.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the Name of the database.
        /// </summary>
        /// <value>The name of the catalog.</value>
        public string CatalogName
        {
            get { return this.adomdConnectionObjMain.Database; }
        }

        /// <summary>
        /// Gets the current cell set.
        /// </summary>
        /// <value>The current cell set.</value>
        [XmlIgnore]
        public CellSet CurrentCellSet
        {
            get { return this.cellSet; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Disconnect();
            adomdConnectionObjMain = null;
        }

        #endregion
    }
}
