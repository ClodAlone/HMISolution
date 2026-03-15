#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.OlapSilverlight.Wrapper;
using Syncfusion.Olap.Manager;
using Syncfusion.OlapSilverlight.Reports;

namespace Syncfusion.OlapSilverlight.Manager
{
    /// <summary>
    /// Manages the data related information.
    /// </summary>
    public class OlapDataProvider : IOlapDataProvider, IDisposable
    {
        #region Initilize/Fanilize

        /// <summary>
        /// Initializing the manager with the connection string
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OlapDataProvider(string connectionString)
        {
            this.DataProvider = new Syncfusion.Olap.DataProvider.AdomdDataProvider(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="isNonSSAS">if set to <c>true</c> [is non SSAS].</param>
        [Obsolete("Please use OlapDataProvider(string connectionString, Syncfusion.Olap.DataProvider.Providers providerName) method.")]
        public OlapDataProvider(string connectionString, bool isNonSSAS)
        {
            this.DataProvider = new Syncfusion.Olap.DataProvider.AdomdDataProvider(connectionString);
            if (isNonSSAS)
            {
                this.DataProvider.ProviderName = Olap.DataProvider.Providers.Mondrian;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        public OlapDataProvider(string connectionString, Syncfusion.Olap.DataProvider.Providers providerName)
        {
            this.DataProvider = new Syncfusion.Olap.DataProvider.AdomdDataProvider(connectionString);
            this.DataProvider.ProviderName = providerName;
        }
        /// <summary>
        /// Initializing the manager with the provider
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public OlapDataProvider(Syncfusion.Olap.DataProvider.IDataProvider dataProvider)
        {
            this.DataProvider = dataProvider;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [Before MDX query execute].
        /// </summary>
        public event QueryExecuteEventHandler BeforeMdxQueryExecute;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value to display the Localized member property in the output
        /// </summary>
        /// <value>
        /// <c>true</c> display the Localized member property; otherwise, <c>false</c>
        /// </value>
        public bool ShowLocalizedMemberProperties
        {
            get;
            set;
        }

        private Items _virtualKpiElements;
        /// <summary>
        /// Gets or sets the virtual KPI collection.
        /// </summary>
        public Items VirtualKpiElements
        {
            get { return _virtualKpiElements; }
            set { _virtualKpiElements = value; }
        }

        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>
        /// <value>The data provider.</value>
        public Syncfusion.Olap.DataProvider.IDataProvider DataProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is non SSAS.
        /// </summary>
        /// <value>
        ///    <c>true</c> if this instance is non SSAS; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This property is no longer exist.")]
        public bool IsNonSSAS { get; private set; }

        #endregion

        #region IOlapDataProvider Members

        /// <summary>
        /// Processes the OlapReport and Generates the CellSet
        /// </summary>
        /// <param name="olapReport"></param>
        /// <returns>A CellSet.</returns>
        public Syncfusion.OlapSilverlight.Data.CellSet ExecuteOlapReport(Syncfusion.OlapSilverlight.Reports.OlapReport olapReport)
        {
            return this.ExecuteOlapReportWithLevelTypeAll(olapReport, false);
        }

        /// <summary>
        /// Executes the OlapReport with level type all.
        /// </summary>
        /// <param name="olapReport">The OlapReport.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <returns>A CellSet.</returns>
        public Syncfusion.OlapSilverlight.Data.CellSet ExecuteOlapReportWithLevelTypeAll(Syncfusion.OlapSilverlight.Reports.OlapReport olapReport, bool showLevelTypeAll)
        {
            if (this.DataProvider != null)
            {
                //// Generating a base OlapReport object form the Silverlight report
                Syncfusion.Olap.Reports.OlapReport report = OlapReportWrapper.GetOlapReportFromWrapper(olapReport);
                //// Instantiating the OLAP manager by passing the Data provider
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider) { ShowLevelTypeAll = showLevelTypeAll };
                odm.SetCurrentReport(report);
                odm.ShowLocalizedMemberProperties = this.ShowLocalizedMemberProperties;
                BindAndRaiseEvent(odm);
                Syncfusion.Olap.Data.CellSet olapCellSet = odm.ExecuteCellSet();

                if (olapCellSet != null)
                {
                    Syncfusion.Olap.Data.CellCollection cellCollection = new Syncfusion.Olap.Data.CellCollection();
                    Syncfusion.OlapSilverlight.Data.CellSet cellSet = CellSetWrapper.GetCellSetWrapper(olapCellSet, cellCollection.GetCells(olapCellSet));
                    return cellSet;
                } 
            }
            return null;
        }

        /// <summary>
        /// Executes the olap report on drill state settings enabled.
        /// </summary>
        /// <param name="olapReport">The olap report.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <returns></returns>
        public Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object> ExecuteOlapReportOnDrillState(Syncfusion.OlapSilverlight.Reports.OlapReport olapReport, bool showLevelTypeAll)
        {
            if (this.DataProvider != null)
            {
                var dictionary = new Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object>();
                //// Generating a base OlapReport object form the Silverlight report
                Syncfusion.Olap.Reports.OlapReport report = OlapReportWrapper.GetOlapReportFromWrapper(olapReport);
                //// Instantiating the OLAP manager by passing the Data provider
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider) { ShowLevelTypeAll = showLevelTypeAll };
                odm.SetCurrentReport(report);
                BindAndRaiseEvent(odm);
                Syncfusion.Olap.Data.CellSet olapCellSet = odm.ExecuteCellSet();

                if (olapCellSet != null)
                {
                    Syncfusion.Olap.Data.CellCollection cellCollection = new Syncfusion.Olap.Data.CellCollection();
                    dictionary["CellSet"] = CellSetWrapper.GetCellSetWrapper(olapCellSet, cellCollection.GetCells(olapCellSet));
                    dictionary["OlapReport"] = OlapReportWrapper.GetOlapReportWrapper(odm.CurrentReport);
                    return dictionary;
                }
            }
            return null;
        }

        /// <summary>
        /// Executes the MDX query.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <returns>A CellSet.</returns>
        public Syncfusion.OlapSilverlight.Data.CellSet ExecuteMdxQuery(string mdxQuery)
        {
            if (this.DataProvider != null)
            {
                 //// Instantiating the data manager by passing the Data provider
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                Syncfusion.Olap.Data.CellSet olapCellSet = odm.ExecuteCellSet(mdxQuery);
                Syncfusion.Olap.Data.CellCollection cellCollection = new Syncfusion.Olap.Data.CellCollection();
                Syncfusion.OlapSilverlight.Data.CellSet cellSet = CellSetWrapper.GetCellSetWrapper(olapCellSet, cellCollection.GetCells(olapCellSet));
                return cellSet;
            }
            return null;            
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>An object.</returns>
        public object Execute(string query)
        {
            if (this.DataProvider != null)
            {
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                BindAndRaiseEvent(odm);
                var resultSet = odm.Execute(query);
                return resultSet;
            }

            return null;
        }

        /// <summary>
        /// Gets the cubes.
        /// </summary>
        /// <returns>Cube information as a collection.</returns>
        public Data.CubeInfoCollection GetCubes()
        {
            if (this.DataProvider != null)
            {
                //// Instantiating the olap manager by passing the Data provider
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                //// Getting collection of cubes in the connected server
                Syncfusion.OlapSilverlight.Data.CubeInfoCollection m_cubeCollection = CubeSchemaWrapper.GetCubes(odm.DataProvider.GetCubes);
                return m_cubeCollection;              
            }
            return null;
        }

        /// <summary>
        /// Gets the cube schema.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>Cube schema information.</returns>
        public Data.CubeSchema GetCubeSchema(string cubeName)
        {
            if (this.DataProvider != null && !string.IsNullOrEmpty(cubeName))
            {
                //// Instantiating the olap manager by passing the Data provider
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                //// Getting cube schema of the given cube
                Syncfusion.OlapSilverlight.Data.CubeSchema m_cubeSchema = CubeSchemaWrapper.GetCubeSchema(odm.DataProvider.GetCubeSchema(cubeName));
                return m_cubeSchema;
            }
            return null;
        }

        /// <summary>
        /// Gets the level members.
        /// </summary>
        /// <param name="levelUniqueName">Name of the level unique.</param>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>Member collection.</returns>
        public Data.MemberCollection GetLevelMembers(string levelUniqueName, string cubeName)
        {
            if (this.DataProvider != null && !string.IsNullOrEmpty(levelUniqueName) && !string.IsNullOrEmpty(cubeName))
            {
                //// Instantiating the olap manager by passing the Data provider
                Syncfusion.Olap.Manager.OlapDataManager olapManager = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                //// Getting Level member collection for the given level
                Syncfusion.OlapSilverlight.Data.MemberCollection m_memberCollection = CubeSchemaWrapper.GetMemberCollection(olapManager.DataProvider.GetLevelMembers(levelUniqueName, cubeName), null, true);

                return m_memberCollection;
            }
            return null;
        }

        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <param name="memberUniqueName">Name of the member unique.</param>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>Member collection.</returns>
        public Data.MemberCollection GetChildMembers(string memberUniqueName, string cubeName)
        {
            if (this.DataProvider != null && !string.IsNullOrEmpty(memberUniqueName) && !string.IsNullOrEmpty(cubeName))
            {
                //// Instantiating the olap manager by passing the Data provider
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                Syncfusion.Olap.Data.MemberCollection members = odm.DataProvider.GetChildMembers(memberUniqueName, cubeName, false);
                Syncfusion.OlapSilverlight.Data.MemberCollection m_memberCollection = CubeSchemaWrapper.GetMemberCollection(members, null, false);

                return m_memberCollection;
            }
            return null;
        }

        public Data.MemberCollection GetChildrenByMDX(string commandText)
        {
            if (this.DataProvider != null && !string.IsNullOrEmpty(commandText) )
            {
                //// Instantiating the olap manager by passing the Data provider
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                Syncfusion.Olap.Data.MemberCollection members = odm.DataProvider.GetChildrenByMDX(commandText);
                Syncfusion.OlapSilverlight.Data.MemberCollection m_memberCollection = CubeSchemaWrapper.GetMemberCollection(members, null, false);

                return m_memberCollection;
            }
            return null;
        }

        /// <summary>
        /// Executes the OlapReport with total count.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <returns>Combination of CellSet and Row count as well Column count.</returns>
        public Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object> ExecuteOlapReportWithTotalCount(Reports.OlapReport report)
        {
            if (this.DataProvider != null)
            {
                var dict = new Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object>();
                var olapReport = OlapReportWrapper.GetOlapReportFromWrapper(report);
                var odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                odm.SetCurrentReport(olapReport);
                BindAndRaiseEvent(odm);
                var counts = odm.ExecuteCount();

                if (counts != null && EnsureCurrentPageAndTotalCount(report, counts))
                {
                    olapReport.PagerOptions.CategorialCurrentPage = Math.Max(1, (int)Math.Ceiling((double)counts["Column"] / report.PagerOptions.CategorialPageSize));
                    olapReport.PagerOptions.SeriesCurrentPage = Math.Max(1, (int)Math.Ceiling((double)counts["Row"] / report.PagerOptions.SeriesPageSize));
                }
                var olapCellSet = odm.ExecuteCellSet();
                if (olapCellSet != null)
                    dict["CellSet"] = CellSetWrapper.GetCellSetWrapper(olapCellSet, new Olap.Data.CellCollection().GetCells(olapCellSet));

                dict["Count"] = counts;

                return dict;
            }
            return null;
        }

        /// <summary>
        /// Gets the MDX query.
        /// </summary>
        /// <param name="olapReport">The OlapReport.</param>
        /// <returns>MDX Query as a string.</returns>
        public string GetMdxQuery(Syncfusion.OlapSilverlight.Reports.OlapReport olapReport)
        {
            if (this.DataProvider != null)
            {
                //// Generating a base OlapReport object form the silverlight report
                Syncfusion.Olap.Reports.OlapReport report = OlapReportWrapper.GetOlapReportFromWrapper(olapReport);
                //// Instantiating the olap manager by passing the Dataprovider
                Syncfusion.Olap.Manager.OlapDataManager odm = new Syncfusion.Olap.Manager.OlapDataManager(DataProvider as Syncfusion.Olap.DataProvider.AdomdDataProvider);
                odm.SetCurrentReport(report);
                return odm.GetMDXQuery();
            }
            return null;
        }
        #endregion

        bool EnsureCurrentPageAndTotalCount(Reports.OlapReport report, Syncfusion.Olap.Common.SerializableDictionary<string, int> count)
        {
            return (count["Column"] < (report.PagerOptions.CategorialCurrentPage - 1) * report.PagerOptions.CategorialPageSize || count["Row"] < (report.PagerOptions.SeriesCurrentPage - 1) * report.PagerOptions.SeriesPageSize);
        }

        void BindAndRaiseEvent(Syncfusion.Olap.Manager.OlapDataManager odm)
        {
            odm.BeforeMdxQueryExecute += ((sender, args) =>
                {
                    if (this.BeforeMdxQueryExecute != null)
                        this.BeforeMdxQueryExecute(this, args);
                });
        }


        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.DataProvider.CloseConnection();
        } 

        #endregion
    }
}