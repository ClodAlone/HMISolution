using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataReaderUnitTests.Helpers;
using System.Collections.Generic;
using Microsoft.Data.ConnectionUI;
using DataReader.Extensions;
using System.Data;
using DataReader.SchemaInfo;
using System.Configuration.Provider;
using System.Globalization;

namespace DataReaderUnitTests
{
    [TestClass]
    public class ProviderMySqlDataReaderTests
    {
        [TestMethod]
        public void TestMySqlClientProviderSchemaInfo()
        {
            if (!FactoryHelper.IsProviderAviable(DataProvider.MySQLDataProvider.Name))
                return;

            DataReader.SchemaInfo.DbSchemaInfo schemaInfo = null;
            try
            {
                schemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(DataProvider.MySQLDataProvider.Name,
                    @"server=localhost;user id=root;password=1-1-91;persist security info=true;CharSet=utf8");
            }
            catch { }
            if (schemaInfo == null)
                return;

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
