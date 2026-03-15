using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Utilities.ImportExportUtils
{
    public class ImportExport
    {
        public static string Delimiter { get { return "\t"; } }
        public static string CsvSeparator { get { return ";"; } }
        public static string NewLine { get { return "\r\n"; } }


        public static int ImportFromFile(string filename, List<object> addlist,ref string[] colnames, string separator)
        {
            using (StreamReader readFile = new StreamReader(filename))
            {
                string line;
                bool firstline = true;
                string wholefile = readFile.ReadToEnd();
                var alllines = wholefile.Split(new string[] { NewLine }, StringSplitOptions.None);
                for (int k = 0; k < alllines.Length; k++)
                //while ((line = readFile.ReadLine()) != null)
                {
                    line = alllines[k];
                    string[] cols = firstline ? line.Split(new string[1] { separator }, StringSplitOptions.RemoveEmptyEntries) : line.Split(new string[1] { separator }, StringSplitOptions.None);
                    if (firstline)
                    {
                        //cols identifiers
                        firstline = false;
                        if(colnames == null || colnames.Length < cols.Length)
                            colnames = new string[cols.Length];
                        cols.CopyTo(colnames, 0);
                    }
                    else
                        addlist.Add(cols);
                }
            }
            return 0;
        }

        public static int ImportFromFile(string filename, List<object> addlist, ref string[] colnames)
        {
            return ImportFromFile(filename, addlist, ref colnames, CsvSeparator);
        }
    }
}
