using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataReaderUnitTests.Helpers;
using System.Collections.Generic;
using Microsoft.Data.ConnectionUI;
using System.Data;
using DataReader.Extensions;

namespace DataReaderUnitTests
{
    [TestClass]
    public class OdbcOracleDataReaderTests
    {
        [TestMethod]
        public void TestOracleOdbcSchemaInfo()
        {
            using (var config = new OracleConfigDataSource() { UserName = "SYSTEM", Password = "1-1-91" })
            {
                if (!config.Add())
                    Assert.Inconclusive("Cannot create ODBC DSN test connection! Ensure to have installed the ODBC driver '{0}'", config.DriverName);
                var schemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(DataProvider.OdbcDataProvider.Name, config.ConnectionString);
                Assert.AreEqual(schemaInfo.DataProviderName, DataProvider.OdbcDataProvider.Name);
                
                var odbcTypes = Enum.GetValues(typeof(DbType));
                foreach (DbType type in odbcTypes)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("------  DataType SchemaInfo for '{0}' ------", type));
                    var typeInfo = schemaInfo.GetProviderTypeInfo(type.ToNetType());
                    if (typeInfo != null)
                    {
                        foreach (var info in typeInfo)
                        {
                            System.Diagnostics.Trace.WriteLine(String.Format("ColumnName = {0}, Value = {1}", info.Key, info.Value));
                        }
                    }
                }
            }
        }
    }
}
