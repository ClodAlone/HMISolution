using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderUnitTests.Helpers
{
    public class MsSqlConfigDataSource : OdbcConfigDataSourceBase
    {
        #region Declarations
        readonly string driverName;
        #endregion

        #region Constructors
        public MsSqlConfigDataSource(string dataSourceName, DataSourceType type, string driverVersion) :
            base(dataSourceName, type)
        {
            this.driverName = String.Format("SQL Server Native Client {0}", driverVersion);

            SetDefaultProperies();
        }

        public MsSqlConfigDataSource(string driverVersion)
        {
            this.driverName = String.Format("SQL Server Native Client {0}", driverVersion);

            SetDefaultProperies();
        }

        void SetDefaultProperies()
        {
            ServerName = "(local)";
        }
        #endregion

        #region Overrides
        public override bool Add()
        {
            var parameters = PrepareParameters();
            return SQLConfigDataSource((IntPtr)0, (int)AddAttribute, driverName, parameters);
        }

        public override bool Modify()
        {
            var parameters = PrepareParameters();
            return SQLConfigDataSource((IntPtr)0, (int)ModifyAttribute, driverName, parameters);
        }

        public override bool Remove()
        {
            var parameters = PrepareParameters();
            return SQLConfigDataSource((IntPtr)0, (int)RemoveAttribute, driverName, parameters);
        }

        protected override string PrepareParameters()
        {
            var parameters = new StringBuilder(base.PrepareParameters());
            parameters.AppendFormat("\0Server={0}", ServerName);
            if (!String.IsNullOrEmpty(DataBaseName))
                parameters.AppendFormat("\0Database={0}", DataBaseName);
            if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password))
                parameters.AppendFormat("\0UserName={0}\0Password={1}", UserName, Password);
            else
                parameters.Append("\0Trusted_Connection=Yes");

            return parameters.ToString();
        }
        #endregion

        #region Properties
        public String DriverName
        {
            get
            {
                return driverName;
            }
        }

        public String ServerName { get; set; }

        public String DataBaseName { get; set; }

        public String UserName { get; set; }

        public String Password { get; set; }
        #endregion
    }
}
