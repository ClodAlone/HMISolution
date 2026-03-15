using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConverterModel
{
    public class ExportUtils
    {

        public const string delimiter = "\t";
        public const string csvseparator = ";";
        public const string newline = "\r\n";
        public const string ciseparator = "@";

        public static int ImportFromFile(string filename, List<object> addlist, ref string[] colnames)
        {
            using (StreamReader readFile = new StreamReader(filename))
            {
                string line;
                bool firstline = true;
                string wholefile = readFile.ReadToEnd();
                var alllines = wholefile.Split(new string[] { UnitConverterModel.ExportUtils.newline }, StringSplitOptions.None);
                for (int k = 0; k < alllines.Length; k++)
                //while ((line = readFile.ReadLine()) != null)
                {
                    line = alllines[k];
                    string[] cols = line.Split(new string[1] { UnitConverterModel.ExportUtils.csvseparator }, StringSplitOptions.None);
                    if (firstline)
                    {
                        //cols identifiers
                        firstline = false;
                        if (colnames == null || colnames.Length < cols.Length)
                            colnames = new string[cols.Length];
                        cols.CopyTo(colnames, 0);
                    }
                    else
                        addlist.Add(cols);
                }
            }
            return 0;
        }
    }
}
