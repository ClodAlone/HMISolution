using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderUnitTests.Helpers
{
    public abstract class OdbcConfigDataSourceBase : IDisposable
    {
        #region Declarations
        readonly string dataSourceName;
        readonly DataSourceType type;
        #endregion

        #region DllImports
        [DllImport("ODBCCP32.dll")]
        protected static extern bool SQLConfigDataSource(IntPtr hwndParent, int fRequest, string lpszDriver, string lpszAttributes);
        #endregion


        #region Constructors
        public OdbcConfigDataSourceBase() : 
            this("MyDSN", DataSourceType.User)
        { }

        protected OdbcConfigDataSourceBase(string dataSourceName, DataSourceType type)
        {
            this.dataSourceName = dataSourceName;
            this.type = type;
        }
        #endregion

        #region Methods
        public abstract bool Add();

        public abstract bool Modify();

        public abstract bool Remove();

        protected virtual string PrepareParameters()
        {
            var parameters = new StringBuilder(String.Format("DSN={0}", dataSourceName));
            return parameters.ToString();
        }
        #endregion

        #region Properties
        public String ConnectionString
        {
            get
            {
                return String.Format("Dsn={0}", dataSourceName);
            }
        }

        protected RequestType AddAttribute
        {
            get
            {
                return type == DataSourceType.User ? RequestType.AddUserDataSource : RequestType.AddSystemDataSource;
            }
        }

        protected RequestType ModifyAttribute
        {
            get
            {
                return type == DataSourceType.User ? RequestType.ModifyUserDataSource : RequestType.ModifySystemDataSource;
            }
        }

        protected RequestType RemoveAttribute
        {
            get
            {
                return type == DataSourceType.User ? RequestType.RemoveUserDataSource : RequestType.RemoveSystemDataSource;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Remove();
        }
        #endregion
    }
}
