using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.ConnectionUI
{
    public class PostgreSQLConnectionProperties : AdoDotNetConnectionProperties
    {
        public PostgreSQLConnectionProperties()
            : base("PostgreSQL.Data.PostgreSQLClient")
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
                if (!(ConnectionStringBuilder["Host"] is string) ||
                    (ConnectionStringBuilder["Host"] as string).Length == 0)
                {
                    return false;
                }
                if (!(ConnectionStringBuilder["Database"] is string) ||
                    (ConnectionStringBuilder["Database"] as string).Length == 0)
                {
                    return false;
                }
                if (!(bool)ConnectionStringBuilder["Integrated Security"] &&
                    (!(ConnectionStringBuilder["UserName"] is string) ||
                    (ConnectionStringBuilder["UserName"] as string).Length == 0))
                {
                    return false;
                }
                return true;
            }
        }
                
        private void LocalReset()
        {
            ConnectionStringBuilder.Add("Persist Security Info", false);
            ConnectionStringBuilder.Add("Timezone", "UTC");
            //this["Integrated Security"] = true;
        }
    }
}
