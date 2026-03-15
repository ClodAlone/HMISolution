using System;
using System.Collections.Generic;

namespace DICom
{
    public class ImportFromFile
    {
        public List<DIComProtocol.DiComVar> ParsedVars;

        public ImportFromFile()
        {
            ParsedVars = new List<DIComProtocol.DiComVar>();
        }

        private bool GetNextOfCsvLine(string line, ref int position, out string result)
        {            
            int newPosition = line.IndexOf(',', position);
            if (newPosition != -1)
            {
                result = line.Substring(position, newPosition - position);
                position = newPosition + 1;
                return true;
            }
            else
            {
                result = null;
                position = newPosition + 1;
                return false;
            }            
        }

        private bool ParseCsvLine(string line, out DIComProtocol.DiComVar var)
        {
            int position = 0;
            string linePart = null;

            var = new DIComProtocol.DiComVar();

            //GENERAL RECORD DESCRIPTION  --> ignore value
            if (!GetNextOfCsvLine(line, ref position, out linePart))
                return false;

            //VARIABLE INDEX --> ignore value
            if (!GetNextOfCsvLine(line, ref position, out linePart))
                return false;

            //TYPE
            if (!GetNextOfCsvLine(line, ref position, out linePart))
                return false;
                        
            if (!Enum.TryParse(linePart, true, out DIComProtocol.DiCommVarType type))
                return false;
            else
                var.VarType = (DIComProtocol.DiCommVarType)type;

            //SIZE --> ignore --> driver get this parameter value
            if (!GetNextOfCsvLine(line, ref position, out linePart))
                return false;

            //TAG
            if (!GetNextOfCsvLine(line, ref position, out linePart))
                return false;
            if (string.IsNullOrWhiteSpace(linePart))
                return false;
            else
                var.VarName = linePart.Trim();

            //NOTES
            if (!GetNextOfCsvLine(line, ref position, out linePart))
                return false;
            var.Note = linePart.Trim();
            
            // EXAMPLE, UNIT --> ignore these fields
            return true;
        }

        public string GetTagName(DIComProtocol.DiComVar var)
        {
            if (!string.IsNullOrEmpty(var.Note))
                return DIComProtocol.CorrectVarNameToMoviconVariableName(var.Note);
            else
                return DIComProtocol.CorrectVarNameToMoviconVariableName(var.VarName);
        }

        public bool ParseCsv(string file, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                {
                    string line;
                    ParsedVars.Clear();
                    while ((line = readFile.ReadLine()) != null)
                    {
                        if (ParseCsvLine(line, out DIComProtocol.DiComVar var))
                        {
                            if (var != null)
                                ParsedVars.Add(var);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            
            return string.IsNullOrEmpty(errorMessage);
        }
    }
}
