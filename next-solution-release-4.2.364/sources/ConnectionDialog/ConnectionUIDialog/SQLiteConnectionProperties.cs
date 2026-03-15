using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.ConnectionUI
{
    public class SQLiteConnectionProperties : AdoDotNetConnectionProperties
    {
        public SQLiteConnectionProperties()
            : base("System.Data.SQLite")
        {
            LocalReset();
        }

        public override void Reset()
        {
            base.Reset();
            LocalReset();
        }

        public override bool IsComplete
        {
            get
            {
                if (!(ConnectionStringBuilder["DataSource"] is string) ||
                    (ConnectionStringBuilder["DataSource"] as string).Length == 0)
                {
                    return false;
                }

                return true;
            }
        }

        protected override string ToTestString()
        {
            return ConnectionStringBuilder.ConnectionString; 
        }

        private void LocalReset()
        {
            ConnectionStringBuilder.Add("DataSource", String.Empty);
            ConnectionStringBuilder.Add("DateTimeKind", "Utc");
            //ConnectionStringBuilder.Add("Password", String.Empty);
            //this["Integrated Security"] = true;
        }
    }
}
