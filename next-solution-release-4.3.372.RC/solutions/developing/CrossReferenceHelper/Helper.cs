using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UFUAEditor.ComponentService;

namespace CrossReferenceHelper
{
    public class Helper
    {
        static List<string> sKeyList;
        public static string GetNewPath(string shortname, string readablePath, string rootName = null, UInt16? ns = null)
        {
            if (ns == null)
                ns = GetNSNumber();
            if (rootName == null)
                rootName = UFUAServerInfo.UFUAServerInfo.GetTagRootName();
            string tagsprefix = readablePath != null && readablePath.StartsWith($"{ns}:{rootName}/") ? $"{ns}:{rootName}/" : string.Empty;
            shortname = shortname.Replace("\\", "/").Replace(":", "/");
            if (shortname.Contains('/'))
            {
                System.Text.StringBuilder rel = new System.Text.StringBuilder();
                foreach (String s in shortname.Split('/').ToList())
                {
                    rel.Append($"{ns}:{s}/");
                }
                readablePath = rel.ToString().Remove(rel.Length - 1);
            }
            else
            {
                readablePath = String.Format("{1}:{0}", shortname, ns);
            }

            readablePath = $"{tagsprefix}{readablePath}";
            return readablePath;
        }
        public static UInt16 GetNSNumber()
        {
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            return (UInt16)(n.Count + 2 - 1);
        }
        static string GetAssemblyPath()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string s = a.Location.ToLower();
            string name = a.GetName().Name.ToLower();
            int idx = s.LastIndexOf(name);
            if (idx != -1)
                return a.Location.Substring(0, idx);
            return string.Empty;
        }
        public static String GetBasicKeywordsListFile()
        {
            return String.Format("{0}\\BasicKeywords.xml", GetAssemblyPath());
        }
        public static int ReplaceVariableInBasicScript(IDocument doc, ref String szCode, string szOldVar, string szNewVar)
        {
            if (!string.IsNullOrEmpty(szNewVar) && szNewVar.Contains("\\"))
                szNewVar = szNewVar.Replace('\\', '_');

            if (string.IsNullOrEmpty(szOldVar) || szNewVar == szOldVar)
                return 0;

            IUFUAEditorManager uFUAEditorManager = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (uFUAEditorManager == null)
                return 0;
            int nReplaced = 0;
            if(szOldVar.Contains("-"))
                szOldVar = szOldVar.Replace("-", ".");
            if(szNewVar.Contains(":"))
                szNewVar = szNewVar.Replace(":", ".");

            List<string> sList = new List<string>();
            string szWhole = szCode;
            int nRow = szWhole.IndexOf("\r\n");
            string szRowToAdd;
            while (nRow != -1)
            {
                szRowToAdd = szWhole.Substring(0, nRow + 2);
                sList.Add(szRowToAdd);
                szWhole = szWhole.Substring(nRow + 2);
                nRow = szWhole.IndexOf("\r\n");
            }
            if (!string.IsNullOrEmpty(szWhole))
            {
                sList.Add(szWhole);
            }
            if (sKeyList == null)
                sKeyList = new List<string>();
            if (sKeyList.Count == 0)
            {
                var bKeys = XElement.Load(GetBasicKeywordsListFile());
                var keyList = from item in bKeys.Descendants("Key")
                             select item.Value;
                if(keyList != null)
                    sKeyList.AddRange(keyList);
            }

            List<string> sOutList = new List<string>();
            int nComment;
            for (int pos = 0; pos < sList.Count; pos++)
            {
                szRowToAdd = sList[pos];
                nComment = szRowToAdd.IndexOf('\'');
                if (nComment != -1 && szRowToAdd.Substring(0, nComment).IndexOf(szOldVar) == -1)
                {
                    sOutList.Add(szRowToAdd);
                    continue;
                }
                nComment = (nComment != -1 ? nComment : szRowToAdd.Length);
                int nVar = szRowToAdd.Substring(0, nComment).IndexOf(szOldVar);
                int nLen = szOldVar.Length;
                int nNewLen = szNewVar.Length;
                bool bChange = false;

                if (nVar != -1)
                {
                    do
                    {
                        string szCutRight = szRowToAdd.Substring(nVar + nLen, nComment - (nVar + nLen)).TrimStart();
                        string szCutLeft = szRowToAdd.Substring(0, nVar).TrimEnd();
                        if ((nVar > 0 && (nVar + nLen < szRowToAdd.Length)) &&
                            (szRowToAdd[nVar - 1] == '"' &&
                            (szRowToAdd[nVar + nLen] == '"' || szRowToAdd[nVar + nLen] == '.')))
                        {
                            bChange = true;
                        }
                        else if ((nVar > 0 && (nVar + nLen < szRowToAdd.Length)) && (
                            char.IsLetterOrDigit(szRowToAdd[nVar - 1]) || char.IsLetterOrDigit(szRowToAdd[nVar + nLen])))
                        {
                            bChange = false;
                        }
                        else if ((nVar > 0) && (
                            szRowToAdd[nVar - 1] == ':'
                            || szRowToAdd[nVar - 1] == '_'
                            || szRowToAdd[nVar - 1] == '.'))
                        {
                            //No
                            bChange = false;
                        }
                        else if ((nVar + nLen < szRowToAdd.Length) && (
                            szRowToAdd[nVar + nLen] == ':'
                            || (szRowToAdd[nVar + nLen] == '_')
                            || szRowToAdd[nVar + nLen] == '.'))
                        {
                            int nLenght = szCutRight.Length;
                            string key = szRowToAdd.Substring(nVar, nLen);
                            string tagfound;
                            for (int i = 0; i < nLenght; i++)
                            {
                                key += szCutRight[i];
                                if (key.Contains("."))
                                    key = key.Replace(".", "_");
                                //find a valid name
                                if (!UFUAModel.Helpers.NameValidator.IsValidName(key))
                                {
                                    key = key.Substring(0, key.Length - 1);
                                    break;
                                }
                            }
                            if(key.Contains("_"))
                                key = key.Replace("_", "\\");
                            bChange = uFUAEditorManager.CheckVariable(doc, key, null, null, out tagfound);
                        }
                        else if (szRowToAdd.Substring(nVar + nLen, nComment - (nVar + nLen)).TrimStart()[0] == '(')
                        {
                            bChange = false;
                        }
                        else if (string.IsNullOrEmpty(szCutRight) && string.IsNullOrEmpty(szCutLeft))
                        {
                            bChange = true;
                        }
                        else if (
                            //following
                            szCutRight.StartsWith("+")
                            || szCutRight.StartsWith("-")
                            || szCutRight.StartsWith("=")
                            || szCutRight.StartsWith("|")
                            || szCutRight.StartsWith("&")
                            || szCutRight.StartsWith("<")
                            || szCutRight.StartsWith(">")
                            || szCutRight.StartsWith("/")
                            || szCutRight.StartsWith("\\")
                            || szCutRight.StartsWith("*")
                            || szCutRight.StartsWith("^")
                            || szCutRight.StartsWith("%")
                            || szCutRight.StartsWith(")")
                            || szCutRight.StartsWith("]")
                            || szCutRight.StartsWith("[")
                            || szCutRight.StartsWith(",")
                            //preceeding
                            || szCutLeft.EndsWith("+")
                            || szCutLeft.EndsWith("-")
                            || szCutLeft.EndsWith("=")
                            || szCutLeft.EndsWith("|")
                            || szCutLeft.EndsWith("&")
                            || szCutLeft.EndsWith("<")
                            || szCutLeft.EndsWith(">")
                            || szCutLeft.EndsWith("/")
                            || szCutLeft.EndsWith("\\")
                            || szCutLeft.EndsWith("*")
                            || szCutLeft.EndsWith("^")
                            || szCutLeft.EndsWith("%")
                            || szCutLeft.EndsWith("(")
                            || szCutLeft.EndsWith("[")
                            || szCutLeft.EndsWith(",")
                            )
                        {
                            bChange = true;
                        }
                        else if (szCutRight.StartsWith("#") && szCutLeft.EndsWith("#"))
                        {
                            bChange = true;
                        }
                        else if ((nVar > 0 && (nVar + nLen < szRowToAdd.Length)) &&
                            szRowToAdd[nVar - 1] == ' ' &&
                            (szRowToAdd[nVar + nLen] == ' ' || szRowToAdd[nVar + nLen] == '\r')
                            )
                        {
                            szCutLeft = szRowToAdd.Substring(0, nVar).TrimEnd();
                            int nStart = 0;
                            foreach (var szCompare in sKeyList)
                            {
                                nStart = szCutLeft.Length - szCompare.Length;
                                if (nStart >= 0 && szCutLeft.Substring(nStart).IndexOf(szCompare) != -1)
                                {
                                    bChange = true;
                                    break;
                                }
                            }
                        }

                        if (bChange)
                        {
                            if (szRowToAdd.Substring(nVar, nLen) != szNewVar)
                                szRowToAdd = string.Format("{0}{1}{2}", szRowToAdd.Substring(0, nVar)
                                , szNewVar, szRowToAdd.Substring(nVar + nLen));
                            nComment += (nNewLen - nLen);
                            nReplaced++;
                        }

                        nVar = szRowToAdd.Substring(0, nComment).IndexOf(szOldVar, nVar + (bChange ? nNewLen : nLen));
                    } while (nVar != -1);
                }
                sOutList.Add(szRowToAdd);
            }
            szCode = string.Empty;
            StringBuilder s = new StringBuilder();
            for (int pos = 0; pos < sOutList.Count; pos++)
                s.Append(sOutList[pos]);
            szCode = s.ToString();

            return nReplaced;
        }
    }
}
