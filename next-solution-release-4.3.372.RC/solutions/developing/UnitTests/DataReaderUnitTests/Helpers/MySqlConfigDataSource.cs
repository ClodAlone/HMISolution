using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderUnitTests.Helpers
{
    public enum MySqlDriverType
    {
        ANSI,
        Unicode
    }

    public class MySqlConfigDataSource : OdbcConfigDataSourceBase
    {
        #region Declarations
        readonly string driverName;
        #endregion

        #region Constructors
        public MySqlConfigDataSource(string dataSourceName, DataSourceType type, string driverVersion, MySqlDriverType driverType) :
            base(dataSourceName, type)
        {
            this.driverName = String.Format("MySQL ODBC {0} {1} Driver", driverVersion, driverType);

            SetDefaultProperies();
        }

        public MySqlConfigDataSource(string driverVersion, MySqlDriverType driverType)
        {
            this.driverName = String.Format("MySQL ODBC {0} {1} Driver", driverVersion, driverType);

            SetDefaultProperies();
        }

        void SetDefaultProperies()
        {
            UserName = "root";
            PortNumber = 3306;
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
            if (!String.IsNullOrEmpty(ServerName))
                parameters.AppendFormat("\0SERVER={0}", DataBaseName);
            parameters.AppendFormat("\0PORT={0}", PortNumber);
            if (!String.IsNullOrEmpty(DataBaseName))
                parameters.AppendFormat("\0DATABASE={0}", DataBaseName);
            if (!String.IsNullOrEmpty(UserName))
                parameters.AppendFormat("\0UID={0}", UserName);
            if (!String.IsNullOrEmpty(Password))
                parameters.AppendFormat("\0PWD={0}", Password);
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

        public int PortNumber { get; set; }

        public String UserName { get; set; }

        public String Password { get; set; }
        #endregion
    }
}
