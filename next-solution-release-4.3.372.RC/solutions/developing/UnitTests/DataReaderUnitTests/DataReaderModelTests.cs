using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataReader.Extensions;
using Microsoft.Data.ConnectionUI;

namespace DataReaderUnitTests
{
    [TestClass]
    public class DataReaderModelTests
    {
        [TestMethod]
        public void TestDataReaderModelInitilization()
        {
            string name = "DataSourceSample";
            string connectionString = "XpoProvider=MSSqlServer;data source=(local);Integrated Security=SSPI;initial catalog=DemoMoviconNExT";
            string select = "SELECT [OID],[EventId],[EventType],[SourceNode],[SourceName],[EventDateTime],[EventDateTimeUtc],[EventMessage],[EventDetails],[EventState],[Severity],[EventComment],[UserName],[EventDuration],[EventOccurence],[EventSequence],[RedundancySyncMask],[OptimisticLockField] FROM [daNExT_1288_IOServer].[dbo].[UFUAAuditLogItem]";
            string where = "[OID] = 1";
            string groupby = "[OID], [EventId]";
            string orderby = "[EventId] ASC, [EventType] DESC";

            string query = String.Format("{0} WHERE {1} GROUP BY {2} ORDER BY {3}", select, where, groupby, orderby);

            var dataReader = new DataReader.DataReaderModel(name, connectionString, query);
            Assert.AreEqual(dataReader.DataSourceName, name);
            Assert.AreEqual(dataReader.DataProvider, DataProvider.SqlDataProvider.Name);
            Assert.AreEqual(dataReader.Select, select);
            Assert.AreEqual(dataReader.Where, where);
            Assert.AreEqual(dataReader.GroupBy, groupby);
            Assert.AreEqual(dataReader.Sort, orderby);

            query = String.Format("{0} WHERE {1}", select, where);

            dataReader = new DataReader.DataReaderModel(name, connectionString, query);
            Assert.AreEqual(dataReader.DataSourceName, name);
            Assert.AreEqual(dataReader.DataProvider, DataProvider.SqlDataProvider.Name);
            Assert.AreEqual(dataReader.Select, select);
            Assert.AreEqual(dataReader.Where, where);
            Assert.IsNull(dataReader.GroupBy);
            Assert.IsNull(dataReader.Sort);

            query = String.Format("{0} GROUP BY {1}", select, groupby);

            dataReader = new DataReader.DataReaderModel(name, connectionString, query);
            Assert.AreEqual(dataReader.DataSourceName, name);
            Assert.AreEqual(dataReader.DataProvider, DataProvider.SqlDataProvider.Name);
            Assert.AreEqual(dataReader.Select, select);
            Assert.AreEqual(dataReader.GroupBy, groupby);
            Assert.IsNull(dataReader.Where);
            Assert.IsNull(dataReader.Sort);

            query = String.Format("{0} ORDER BY {1}", select, orderby);

            dataReader = new DataReader.DataReaderModel(name, connectionString, query);
            Assert.AreEqual(dataReader.DataSourceName, name);
            Assert.AreEqual(dataReader.DataProvider, DataProvider.SqlDataProvider.Name);
            Assert.AreEqual(dataReader.Select, select);
            Assert.AreEqual(dataReader.Sort, orderby);
            Assert.IsNull(dataReader.Where);
            Assert.IsNull(dataReader.GroupBy);

            // invalid query1
            query = String.Format("{0} ORDER BY {3} GROUP BY {2} WHERE {1} ", select, where, groupby, orderby);

            try
            {
                dataReader = new DataReader.DataReaderModel(name, connectionString, query);
            }
            catch (ArgumentException)
            { }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            // invalid query2
            query = String.Format("selection TOP 1000 * FROM [Table]", select, where, groupby, orderby);
            try
            {
                dataReader = new DataReader.DataReaderModel(name, connectionString, query);
            }
            catch (ArgumentException)
            { }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
