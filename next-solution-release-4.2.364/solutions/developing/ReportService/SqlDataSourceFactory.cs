using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportManager.ReportService
{
    public static class SqlDataSourceFactory
    {
        public static SqlDataSource CreateSqlDataSource(string connectionString)
        {
            var helper = new ConnectionStringParser(connectionString);
            if (!helper.PartExists(DataStoreBase.XpoProviderTypeParameterName))
                throw new ArgumentException("Missing Provider Type in connectionString argument.");
            
            CustomStringConnectionParameters connectionParameters = new CustomStringConnectionParameters(connectionString);
            return new SqlDataSource(connectionParameters);
        }
    }
}
