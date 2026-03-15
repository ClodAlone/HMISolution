using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataReaderUnitTests.Helpers;
using System.Collections.Generic;
using Microsoft.Data.ConnectionUI;
using DataReader.Extensions;
using System.Data;

namespace DataReaderUnitTests
{
    [TestClass]
    public class ProviderMySqlDataReaderTests
    {
        [TestMethod]
        public void TestMySqlClientProviderSchemaInfo()
        {
            var schemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(DataProvider.MySQLDataProvider.Name,
                @"server=localhost;user id=root;password=1-1-91;persist security info=true;CharSet=utf8");
            Assert.AreEqual(schemaInfo.DataProviderName, DataProvider.MySQLDataProvider.Name);
                
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
