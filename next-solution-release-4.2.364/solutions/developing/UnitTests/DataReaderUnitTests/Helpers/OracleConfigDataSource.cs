using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderUnitTests.Helpers
{
    public class OracleConfigDataSource : OdbcConfigDataSourceBase
    {
        #region Declarations
        readonly string driverName;
        #endregion

        #region Constructors
        public OracleConfigDataSource(string dataSourceName, DataSourceType type) :
            base(dataSourceName, type)
        {
            this.driverName = "Oracle in XE";

            SetDefaultProperies();
        }

        public OracleConfigDataSource()
        {
            this.driverName = "Oracle in XE";

            SetDefaultProperies();
        }

        void SetDefaultProperies()
        {
            ServerName = "XE";
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
            parameters.AppendFormat("\0ServerName={0}", ServerName);
            if (!String.IsNullOrEmpty(UserName))
                parameters.AppendFormat("\0UserID={0}", UserName);
            if (!String.IsNullOrEmpty(Password))
                parameters.AppendFormat("\0Password={0}", Password);
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

        public String UserName { get; set; }

        public String Password { get; set; }
        #endregion
    }
}
