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
    public class ProviderOleDbDataReaderTests
    {
        [TestMethod]
        public void TestSqlOleDbProviderSchemaInfo()
        {
            var schemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(DataProvider.OleDBDataProvider.Name,
                @"Provider=SQLOLEDB;Data Source=(local);Integrated Security=SSPI");
            Assert.AreEqual(schemaInfo.DataProviderName, DataProvider.OleDBDataProvider.Name);

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
