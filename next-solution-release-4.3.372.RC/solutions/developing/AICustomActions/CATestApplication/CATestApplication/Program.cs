using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomActions;

namespace CATestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlInstance = "(local)";

            var domain = "";
            var user = "";
            var password = "";

            bool bError = false;
            try
            {
                CustomActions.CustomActions.InitSessionValues("CUSTOM", "REMOTE", domain, user, password, "TrustedConnection", sqlInstance, "", "");
                bError = CustomActions.CustomActions.Finalize() != WixToolset.Dtf.WindowsInstaller.ActionResult.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                bError = true;
            }

            if (!bError)
                Console.WriteLine("OK");
            else
                Console.WriteLine("Error: {0}", CustomActions.CustomActions.ErrorMessage);
            Console.ReadKey();
        }
    }
}
