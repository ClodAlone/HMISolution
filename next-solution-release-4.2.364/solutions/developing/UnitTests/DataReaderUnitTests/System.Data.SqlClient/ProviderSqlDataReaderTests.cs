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
    public class ProviderSqlDataReaderTests
    {
        [TestMethod]
        public void TestSqlClientProviderSchemaInfo()
        {
            var schemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(DataProvider.SqlDataProvider.Name,
                @"Data Source=(local);Integrated Security=True");
            Assert.AreEqual(schemaInfo.DataProviderName, DataProvider.SqlDataProvider.Name);
                
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
