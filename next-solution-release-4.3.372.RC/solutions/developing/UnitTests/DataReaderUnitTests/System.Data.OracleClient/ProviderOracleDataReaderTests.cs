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
    public class ProviderOracleDataReaderTests
    {
        [TestMethod]
        public void TestOracleClientProviderSchemaInfo()
        {
            if (!FactoryHelper.IsProviderAviable(DataProvider.OracleDataProvider.Name))
                return;

            DataReader.SchemaInfo.DbSchemaInfo schemaInfo = null;
            try
            {
                schemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(DataProvider.OracleDataProvider.Name,
                    @"Data Source=XE;Persist Security Info=True;User ID=SYSTEM;Password=1-1-91;Unicode=True");
            }
            catch { }
            if (schemaInfo == null)
                return;

            Assert.AreEqual(schemaInfo.DataProviderName, DataProvider.OracleDataProvider.Name);
                
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
