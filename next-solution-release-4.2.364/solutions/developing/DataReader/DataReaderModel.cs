using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DevExpress.Xpo.DB.Helpers;

namespace DataReader
{
    public class DataReaderModel : IEquatable<DataReaderModel>
    {
        #region Declarations
        static string dataProviderPartName = "DataProvider";
        static string select = "select ";
        static string where = " where ";
        static string groupby = " group by ";
        static string orderby = " order by ";
        #endregion

        #region Constructors
        public DataReaderModel()
        { }

        public DataReaderModel(DataReaderModel instance)
        {
            if (instance == null)
                return;
            DataProvider = instance.DataProvider;
            Connection = instance.Connection;
            Select = instance.Select;
            Where = instance.Where;
            GroupBy = instance.GroupBy;
            Sort = instance.Sort;
            xmlUri = instance.xmlUri;
            xmlItems = instance.xmlItems;
            DataProviderDescription = instance.DataProviderDescription;
            DataProviderDisplayName = instance.DataProviderDisplayName;
            DataProviderShortDisplayName = instance.DataProviderShortDisplayName;
            DataSourceName = instance.DataSourceName;
            DataSourceDisplayName = instance.DataSourceDisplayName;
            MaxTake = instance.MaxTake;
            TableName = instance.TableName;
            DataColumn = instance.DataColumn;
            TimeColumn = instance.TimeColumn;
        }

        public DataReaderModel(String name, String fullConnection) : 
            this(name, fullConnection, null)
        { }

        public DataReaderModel(String name, String fullConnection, String fullQuery)
        {
            DataSourceName = name;
            if (XpoHelpers.XpoHelper.IsDataSource(fullConnection))
            {
                DataProvider = Helpers.XpoConversionHelper.GetDataProviderFromXpoConnection(fullConnection);
                Connection = Helpers.XpoConversionHelper.GetConnectionStringFromXpoConnection(fullConnection);
            }
            else
            {
                var helper = new ConnectionStringParser(fullConnection);
                DataProvider = helper.GetPartByName(dataProviderPartName);
                helper.RemovePartByName(dataProviderPartName);
                Connection = helper.GetConnectionString();
            }

            if (!String.IsNullOrWhiteSpace(fullQuery))
            {
                var query = fullQuery.ToLower();
                var iwhere = query.IndexOf(where);
                var igroupby = query.IndexOf(groupby);
                var iorderby = query.IndexOf(orderby);

                if (!query.StartsWith(select) || 
                    (igroupby != -1 && iwhere > igroupby) || 
                    (iorderby != -1 && iwhere > iorderby) || 
                    (iorderby != -1 && igroupby > iorderby))
                    throw new ArgumentException("fullQuery");

                if (iwhere != -1)
                {
                    if (Select == null)
                        Select = fullQuery.Substring(0, iwhere);

                    if (igroupby != -1 && igroupby > iwhere)
                        Where = fullQuery.Substring(iwhere + where.Length, igroupby - iwhere - where.Length);
                    else if (iorderby != -1 && iorderby > iwhere)
                        Where = fullQuery.Substring(iwhere + where.Length, iorderby- iwhere - where.Length);
                    else
                        Where = fullQuery.Substring(iwhere + where.Length);

                }

                if (igroupby != -1)
                {
                    if (Select == null)
                        Select = fullQuery.Substring(0, igroupby);

                    if (iorderby != -1 && iorderby > igroupby)
                        GroupBy = fullQuery.Substring(igroupby + groupby.Length, iorderby- igroupby - groupby.Length);
                    else
                        GroupBy = fullQuery.Substring(igroupby + groupby.Length);
                }

                if (iorderby != -1)
                {
                    if (Select == null)
                        Select = fullQuery.Substring(0, iorderby);

                    Sort = fullQuery.Substring(iorderby + orderby.Length);
                }

                if (Select == null)
                    Select = fullQuery;
            }
        }
        #endregion

        #region IEquatable<Memento>
        public bool Equals(DataReaderModel other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return DataProvider == other.DataProvider &&
                Connection == other.Connection &&
                Select == other.Select &&
                Where == other.Where &&
                GroupBy == other.GroupBy &&
                Sort == other.Sort &&
                xmlUri == other.xmlUri &&
                xmlItems == other.xmlItems &&
                DataProviderDescription == other.DataProviderDescription &&
                DataProviderDisplayName == other.DataProviderDisplayName &&
                DataProviderShortDisplayName == other.DataProviderShortDisplayName &&
                DataSourceName == other.DataSourceName &&
                DataSourceDisplayName == other.DataSourceDisplayName &&
                MaxTake == other.MaxTake &&
                TableName == other.TableName &&
                DataColumn == other.DataColumn &&
                TimeColumn == other.TimeColumn;
        }
        #endregion

        #region Members persistance
        [DataMember]
        public String DataProvider;
        [DataMember]
        public String Connection;
        [DataMember]
        public String Select;
        [DataMember]
        public String Where;
        [DataMember]
        public String GroupBy;
        [DataMember]
        public String Sort;

        [DataMember]
        public String xmlUri;
        [DataMember]
        public String xmlItems;

        [DataMember]
        public String DataProviderDescription;
        [DataMember]
        public String DataProviderDisplayName;
        [DataMember]
        public String DataProviderShortDisplayName;

        [DataMember]
        public String DataSourceName;
        [DataMember]
        public String DataSourceDisplayName;

        [DataMember]
        public int MaxTake = 10;

        [DataMember]
        public String TableName;
        [DataMember]
        public String DataColumn;
        [DataMember]
        public String TimeColumn;
        #endregion

        [OnDeserializing]
        private void Initialize(StreamingContext context)
        {
            MaxTake = 10;
        }

        #region Public Methods
        public void NormalizeConnectionString(string projectRoot)
        {
            Connection = XpoHelpers.XpoHelper.NormalizeConnectionString(Connection, projectRoot);
        }
        #endregion
    }
}
