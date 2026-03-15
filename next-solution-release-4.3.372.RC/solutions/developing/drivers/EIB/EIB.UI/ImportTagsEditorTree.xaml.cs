using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace EIB.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>    
    public delegate string GetGroupName(string mainGroup);

    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {        
        bool alreadyLoaded = false;
        public GetStationName readStationName;
        public GetGroupName readGroupName;
        bool forceEISFormat = false;
        EISDATAFORMAT? eisDataFormat = null;
        BaseImportTree baseImportTree;

        public ImportTagsEditorTree()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                {
                    return;
                }

                alreadyLoaded = true;

                List<string> lista = DataContext as List<string>;
                if (lista == null || lista.Count < 2)
                    return;

                Assembly a = Assembly.GetAssembly(this.GetType());
                string DriverName = a.GetName().Name;
                DriverName = DriverName.Replace(".UI", "");

                // Add column to the Import Grid
                List<BaseImportTree.GridColData> columns = new List<BaseImportTree.GridColData>();
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Name",
                    bindingName = "TagName",
                    colWidth = 300
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Address",
                    bindingName = "TagAddress",
                    colWidth = 250
                });                
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 150
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0], (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons(bGetPLCTags: false);
                baseImportTree.SetFileFilter("esf files|*.esf");
                baseImportTree.LoadImportFile = LoadFile;
                                
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                CmbEisDataFormat.Items.Clear();
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFBit, Tag = EISDATAFORMAT.EISDFBit });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFByte, Tag = EISDATAFORMAT.EISDFByte });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFWord, Tag = EISDATAFORMAT.EISDFWord });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFDWord, Tag = EISDATAFORMAT.EISDFDWord });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFFloat, Tag = EISDATAFORMAT.EISDFFloat });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS3, Tag = EISDATAFORMAT.EISDFEIS3 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS4, Tag = EISDATAFORMAT.EISDFEIS4 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS5, Tag = EISDATAFORMAT.EISDFEIS5 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS6, Tag = EISDATAFORMAT.EISDFEIS6 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFAccessPWD6Bytes, Tag = EISDATAFORMAT.EISDFAccessPWD6Bytes });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFAccessPWD10Bytes, Tag = EISDATAFORMAT.EISDFAccessPWD10Bytes });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFInt64, Tag = EISDATAFORMAT.EISDFInt64 });
                                
                readGroupName = (string mainGroup) =>
                {
                    if (AddGroupAddressToVariableName.IsChecked == true)
                        return SplitMainGroup(mainGroup);
                    else
                        return string.Empty;
                };
                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(AddGroupAddressToVariableName, ImportTree_Changed);

                DataContext = this;
            };
        }

        private void ImportTree_Changed(object sender, EventArgs e)
        {
            baseImportTree.UpdateDataWithInterfaceParameters();
        }

        private bool SplitImportAddress(string importAddress, ref string pollingGroup, ref string outputGroup, ref string inputGroups)
        {
            pollingGroup = String.Empty;
            outputGroup = String.Empty;
            inputGroups = String.Empty;

            if (importAddress == String.Empty)
            {
                return (false);
            }

            string[] stringSeparators = new string[] { " - " };
            string[] splitResult = importAddress.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            int groupCounter = splitResult.Count();
            if(groupCounter < 2)
            {
                return (false);
            }

            pollingGroup = splitResult[0];
            outputGroup = splitResult[1];
            if((pollingGroup == String.Empty) || (outputGroup == String.Empty))
            {
                return (false);
            }
            if(groupCounter > 2)
            {
                inputGroups = splitResult[2];
            }


            return (true);
        }

        /// <summary>
        /// Split main group address (Polling) into adress elements
        /// </summary>
        /// <param name="mainGroup"></param>
        /// <param name="mainGroupSplitted"></param>
        /// <returns></returns>
        //private bool SplitMainGroup(string mainGroup, out string[] mainGroupSplitted)
        //{
        //    mainGroupSplitted = mainGroup.Split('/');
        //    if (mainGroupSplitted.Length != 3)
        //        mainGroupSplitted = null;

        //    return (mainGroupSplitted != null);
        //}

        private string SplitMainGroup(string mainGroup)
        {
            string[] mainGroupSplitted = mainGroup.Split('/');
            if (mainGroupSplitted.Length != 3)
                return string.Empty;
            else
                return string.Format("VAR_{0}_{1}_{2}_", mainGroupSplitted[0], mainGroupSplitted[1], mainGroupSplitted[2]);
        }

        public void ImportSelectedTags()
        {                       
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();
            int behaviorExistingTags = baseImportTree.GetBehaviorForExistingTags();
            int behaviorDynamicLink = baseImportTree.GetBehaviorForDynamicLink();

            if (list.Count > 0)
            {
                //string importfolder;
                //if (string.IsNullOrEmpty(ImportFolderText.Text))
                //{
                //    importfolder = CmbStation.Text;
                //}
                //else
                //{
                //    importfolder = ImportFolderText.Text;
                //} 

                if (ForceEISFormat.IsChecked == false)
                {
                    forceEISFormat = false;
                }
                else
                {
                    forceEISFormat = true;
                }                
                if (CmbEisDataFormat.SelectedValue != null)
                    eisDataFormat = (EISDATAFORMAT)(((ComboBoxItem)CmbEisDataFormat.SelectedValue).Tag);
                List<ImportTag> taglist = new List<ImportTag>();
                List<ImportPrototype> protolist = new List<ImportPrototype>();
                foreach (var elem in list)
                {
                    ImportDataEIB single = elem as ImportDataEIB;
                    if (single != null)
                    {                        
                        string pollingGroup = String.Empty;
                        string outputGroup = String.Empty;
                        string inputGroups = String.Empty;
                        if(SplitImportAddress(single.Address, ref pollingGroup, ref outputGroup, ref inputGroups) == false)
                        {
                            continue;
                        }

                        EISDATAFORMAT dataFormat = EISDATAFORMAT.EISDFBit;
                        int moviconType = -1;
                        int arrayDim = 0;
                        GetEisDataFormat(single.szType, single.Name,
                                         ref dataFormat, ref moviconType, ref arrayDim);
                        if(moviconType < 0)
                        {
                            continue;
                        }

                        EIBDynTagSettings tagSettings = new EIBDynTagSettings();
                        tagSettings.StationName = baseImportTree.ReadStationName();
                        tagSettings.EnablePolling = true;
                        tagSettings.PollingGroup = pollingGroup;
                        tagSettings.OutputGroup = outputGroup;
                        tagSettings.InputGroups = inputGroups;
                        tagSettings.DataFormat = (int)dataFormat;

                        single.DynAddress = tagSettings.ToString();
                        single.TagType = (UFUAModel.DataType)moviconType;
                        single.ArrayDimension = (uint)arrayDim;
                        if(dataFormat == EISDATAFORMAT.EISDFEIS3 || dataFormat == EISDATAFORMAT.EISDFEIS4)
                        {
                            //Structure required
                            ImportTag a = null;
                            var proto = new ImportPrototype();

                            proto.Name = (dataFormat == EISDATAFORMAT.EISDFEIS3 ? 
                                Properties.Resources.EIS3StructName : Properties.Resources.EIS4StructName);
                            proto.Elements = new List<ImportTag>();
                            if(dataFormat == EISDATAFORMAT.EISDFEIS3)
                            {
                                a = new ImportTag()
                                {
                                    ModelType = ModelType.Variable,
                                    ArrayDimension = 0,
                                    Name = Properties.Resources.EIS3Member1,
                                    DataType = DataType.Byte,
                                    Description = Properties.Resources.EIS3Member1Desc
                                };
                                proto.Elements.Add(a);
                                a = new ImportTag()
                                {
                                    ModelType = ModelType.Variable,
                                    ArrayDimension = 0,
                                    Name = Properties.Resources.EIS3Member2,
                                    DataType = DataType.Byte,
                                    Description = Properties.Resources.EIS3Member2Desc
                                };
                                proto.Elements.Add(a);
                                a = new ImportTag()
                                {
                                    ModelType = ModelType.Variable,
                                    ArrayDimension = 0,
                                    Name = Properties.Resources.EIS3Member3,
                                    DataType = DataType.Byte,
                                    Description = Properties.Resources.EIS3Member3Desc
                                };
                                proto.Elements.Add(a);
                                a = new ImportTag()
                                {
                                    ModelType = ModelType.Variable,
                                    ArrayDimension = 0,
                                    Name = Properties.Resources.EIS3Member4,
                                    DataType = DataType.Byte,
                                    Description = Properties.Resources.EIS3Member4Desc
                                };
                                proto.Elements.Add(a);
                                if (!protolist.Contains(proto))
                                    protolist.Add(proto);
                            }
                            else
                            {
                                a = new ImportTag()
                                {
                                    ModelType = ModelType.Variable,
                                    ArrayDimension = 0,
                                    Name = Properties.Resources.EIS4Member1,
                                    DataType = DataType.Byte,
                                    Description = Properties.Resources.EIS4Member1Desc
                                };
                                proto.Elements.Add(a);
                                a = new ImportTag()
                                {
                                    ModelType = ModelType.Variable,
                                    ArrayDimension = 0,
                                    Name = Properties.Resources.EIS4Member2,
                                    DataType = DataType.Byte,
                                    Description = Properties.Resources.EIS4Member2Desc
                                };
                                proto.Elements.Add(a);
                                a = new ImportTag()
                                {
                                    ModelType = ModelType.Variable,
                                    ArrayDimension = 0,
                                    Name = Properties.Resources.EIS4Member3,
                                    DataType = DataType.UInt16,
                                    Description = Properties.Resources.EIS4Member3Desc
                                };
                                proto.Elements.Add(a);
                                if (!protolist.Contains(proto))
                                    protolist.Add(proto);
                            }

                            ImportTag tagtoimport = new ImportTag()
                            {
                                Name = single.Name
                                   /*UFUAModel.Helpers.NameValidator.EnsureValidName(
                                                                       )*/,
                                DataType = (DataType)single.TagType,
                                DynSettings = single.DynAddress,
                                Folder = importfolder,
                                ModelType = UFUAModel.ModelType.ObjectType,
                                ArrayDimension = single.ArrayDimension,
                                PrototypeModel = proto.Name,
                                BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                            };

                            taglist.Add(tagtoimport);

                        }
                        else
                        {
                            //as usual
                            ImportTag tagtoimport = new ImportTag()
                            {
                                Name = single.Name
                                    /*UFUAModel.Helpers.NameValidator.EnsureValidName(
                                                                        )*/,
                                DataType = (DataType)single.TagType,
                                DynSettings = single.DynAddress,
                                Folder = importfolder,
                                ModelType = UFUAModel.ModelType.Variable,
                                ArrayDimension = single.ArrayDimension,
                                BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                            };

                            taglist.Add(tagtoimport);
                        }
                        

                    }
                }
                DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
            }
            else
                DataContext = this;
        }       

        private void GetEisDataFormat(string varType, string varDescription,
                                      ref EISDATAFORMAT dataFormat,
                                      ref int moviconType, ref int arrayDim)
        {
            dataFormat = EISDATAFORMAT.EISDFBit;
            moviconType = -1;
            arrayDim = 0;

            if((varType == "EIS 1") ||
               (varType == "EIS 2") ||
               (varType == "EIS 7") ||
               (varType == "EIS 8"))
            {
                dataFormat = EISDATAFORMAT.EISDFBit;
                if ((varType == "EIS 1") || (varType == "EIS 7"))
                {
                    moviconType = (int)DataType.Boolean;
                }
                else
                {
                    moviconType = (int)DataType.Byte;
                }
            }
            else if ((varType == "EIS 13") ||
                     (varType == "EIS 14") ||
                     (varType == "EIS 15"))
            {
                dataFormat = EISDATAFORMAT.EISDFByte;
                moviconType = (int)DataType.Byte;
            }
            else if ((varType == "EIS 10"))
            {
                dataFormat = EISDATAFORMAT.EISDFWord;
                moviconType = (int)DataType.UInt16;
            }
            else if ((varType == "EIS 11") ||
                     (varType == "EIS 12"))
            {
                dataFormat = EISDATAFORMAT.EISDFDWord;
                moviconType = (int)DataType.UInt32;
            }
            else if ((varType == "EIS 9"))
            {
                dataFormat = EISDATAFORMAT.EISDFFloat;
                moviconType = (int)DataType.Float;
            }
            else if ((varType == "EIS 3"))
            {
                dataFormat = EISDATAFORMAT.EISDFEIS3;
                moviconType = (int)DataType.UInt32;
            }
            else if ((varType == "EIS 4"))
            {
                dataFormat = EISDATAFORMAT.EISDFEIS4;
                moviconType = (int)DataType.UInt32;
            }
            else if ((varType == "EIS 5"))
            {
                dataFormat = EISDATAFORMAT.EISDFEIS5;
                moviconType = (int)DataType.Double;
            }
            else if ((varType == "EIS 6"))
            {
                dataFormat = EISDATAFORMAT.EISDFEIS6;
                moviconType = (int)DataType.Byte;
            }
            else if ((varType == "EIS 29"))
            {
                dataFormat = EISDATAFORMAT.EISDFInt64;
                moviconType = (int)DataType.Int64;
            }
            else if((forceEISFormat == true) && eisDataFormat.HasValue)
            {
                dataFormat = (EISDATAFORMAT)eisDataFormat;
                switch (dataFormat)
                {
                    case EISDATAFORMAT.EISDFBit:
                        moviconType = (int)DataType.Boolean;
                    break;

                    case EISDATAFORMAT.EISDFByte:
                        moviconType = (int)DataType.Byte;
                    break;

                    case EISDATAFORMAT.EISDFWord:
                        moviconType = (int)DataType.UInt16;
                    break;

                    case EISDATAFORMAT.EISDFDWord:
                        moviconType = (int)DataType.UInt32;
                    break;

                    case EISDATAFORMAT.EISDFFloat:
                        moviconType = (int)DataType.Float;
                    break;

                    case EISDATAFORMAT.EISDFEIS3:
                        moviconType = (int)DataType.UInt32;
                    break;

                    case EISDATAFORMAT.EISDFEIS4:
                        moviconType = (int)DataType.UInt32;
                    break;

                    case EISDATAFORMAT.EISDFEIS5:
                        moviconType = (int)DataType.Double;
                    break;

                    case EISDATAFORMAT.EISDFEIS6:
                        moviconType = (int)DataType.Byte;
                    break;

                    case EISDATAFORMAT.EISDFAccessPWD6Bytes:
                        moviconType = (int)DataType.Byte;
                        arrayDim = 6;
                    break;

                    case EISDATAFORMAT.EISDFAccessPWD10Bytes:
                        moviconType = (int)DataType.Byte;
                        arrayDim = 10;
                    break;

                    case EISDATAFORMAT.EISDFInt64:
                        moviconType = (int)DataType.Int64;
                        break;
                }
            }
            else if ((varType == "UNCERTAIN (1 BYTE)"))
            {
                dataFormat = EISDATAFORMAT.EISDFByte;
                moviconType = (int)DataType.Byte;
            }
            else if ((varType == "UNCERTAIN (2 BYTE)"))
            {
                dataFormat = EISDATAFORMAT.EISDFEIS5;
                moviconType = (int)DataType.Double;
            }
            else if ((varType == "UNCERTAIN (3 BYTE)"))
            {
                string auxString = varDescription.ToUpper();
                if(auxString.IndexOf("TIME") >= 0)
                {
                    dataFormat = EISDATAFORMAT.EISDFEIS3;
                    moviconType = (int)DataType.UInt32;
                }
                else
                {
                    dataFormat = EISDATAFORMAT.EISDFEIS4;
                    moviconType = (int)DataType.UInt32;
                }
            }
            else if ((varType == "UNCERTAIN (4 BYTE)"))
            {
                dataFormat = EISDATAFORMAT.EISDFFloat;
                moviconType = (int)DataType.Float;
            }
        }

        private void GetEsfVarType(string fileVarType, ref string varType)
        {
            string auxString = fileVarType.ToUpper();
            if(auxString.IndexOf("EIS 1 ") >= 0)
            {
                varType = "EIS 1";
            }
            else if(auxString.IndexOf("EIS 2") >= 0)
            {
                varType = "EIS 2";
            }
            else if (auxString.IndexOf("EIS 3") >= 0)
            {
                varType = "EIS 3";
            }
            else if (auxString.IndexOf("EIS 4") >= 0)
            {
                varType = "EIS 4";
            }
            else if (auxString.IndexOf("EIS 5") >= 0)
            {
                varType = "EIS 5";
            }
            else if (auxString.IndexOf("EIS 6") >= 0)
            {
                varType = "EIS 6";
            }
            else if (auxString.IndexOf("EIS 7") >= 0)
            {
                varType = "EIS 7";
            }
            else if (auxString.IndexOf("EIS 8") >= 0)
            {
                varType = "EIS 8";
            }
            else if (auxString.IndexOf("EIS 9") >= 0)
            {
                varType = "EIS 9";
            }
            else if (auxString.IndexOf("EIS 10") >= 0)
            {
                varType = "EIS 10";
            }
            else if (auxString.IndexOf("EIS 11") >= 0)
            {
                varType = "EIS 11";
            }
            else if (auxString.IndexOf("EIS 12") >= 0)
            {
                varType = "EIS 12";
            }
            else if (auxString.IndexOf("EIS 13") >= 0)
            {
                varType = "EIS 13";
            }
            else if (auxString.IndexOf("EIS 14") >= 0)
            {
                varType = "EIS 14";
            }
            else if (auxString.IndexOf("EIS 15") >= 0)
            {
                varType = "EIS 15";
            }
            else if (auxString.IndexOf("EIS 1") >= 0)
            {
                varType = "EIS 1";
            }
            else if (auxString.IndexOf("UNCERTAIN (1 BYTE)") >= 0)
            {
                varType = "UNCERTAIN (1 BYTE)";
            }
            else if (auxString.IndexOf("UNCERTAIN (2 BYTE)") >= 0)
            {
                varType = "UNCERTAIN (2 BYTE)";
            }
            else if (auxString.IndexOf("UNCERTAIN (3 BYTE)") >= 0)
            {
                varType = "UNCERTAIN (3 BYTE)";
            }
            else if (auxString.IndexOf("UNCERTAIN (4 BYTE)") >= 0)
            {
                varType = "UNCERTAIN (4 BYTE)";
            }
            else if (auxString.IndexOf("UNKNOWN") >= 0)
            {
                varType = "UNKNOWN";
            }
            else
            {
                varType = "UNKNOWN";
            }
        }

        private void GetEsfInputGroups(string fileInputGroups,
                                       ref List<string> inputGroups)
        {
            char[] searchArray = new char[]{' ', '\r', '\n'};
            string groups = fileInputGroups;
            int charIndex = groups.IndexOfAny(searchArray);
            while (charIndex >= 0)
            {
                if((fileInputGroups[charIndex] == '\r') ||
                   (fileInputGroups[charIndex] == '\n'))
                {
                    break;
                }

                string groupAddress = groups.Substring(0, charIndex);
                if(!EIBCommJob.IsValidAddress(groupAddress))
                {
                    if(charIndex == (groups.Length -1))
                    {
                        break;
                    }

                    string auxString = groups.Substring(charIndex + 1);
                    groups = auxString;
                    charIndex = groups.IndexOfAny(searchArray);
                    continue;
                }

                // Add the group to the array of input groups
                if((inputGroups.Count() == 0) ||
                   (inputGroups.Contains(groupAddress) == false))
                {
                    inputGroups.Add(groupAddress);
                }

                string auxString2 = groups.Substring(charIndex + 1);
                groups = auxString2;
                charIndex = groups.IndexOfAny(searchArray);
            }

            // Last group
            if(groups != String.Empty)
            {
                string groupAddress = groups;

                if(EIBCommJob.IsValidAddress(groupAddress))
                {
                    // Add the group to the array of input groups
                    if ((inputGroups.Count() == 0) ||
                        (inputGroups.Contains(groupAddress) == false))
                    {
                        inputGroups.Add(groupAddress);
                    }
                }
            }
       }

        private bool ParseEsfFileLine(string[] tokenizedLine,
                                      ref string mainGroup,
                                      ref string varDescription,
                                      ref string varType,
                                      ref List<string> inputGroups)
        {
            // The number of the fields in the line must be 5
            if (tokenizedLine.Count() != 5)
            {
                return (false);
            }

            // The length of the first field must be at least 5
            if (tokenizedLine[0].Length < 5)
            {
                return (false);
            }

            // Get the main group and check it
            mainGroup = String.Empty;
            string auxString = tokenizedLine[0];
            int pointIndex = auxString.LastIndexOf(".");
            if((pointIndex < 0) || (pointIndex == (auxString.Length - 1)))
            {
                return (false);
            }
            string group = auxString.Substring(pointIndex + 1);
            group.Trim();
            if(group == String.Empty)
            {
                return (false);
            }
            if(!EIBCommJob.IsValidAddress(group))
            {
                return (false);
            }
            mainGroup = group;

            // Get the variable description and substitute invalid characters
            varDescription = String.Empty;
            auxString = tokenizedLine[1];
            auxString.Trim();
            if(auxString != String.Empty)
            {
                varDescription = auxString;
            }

            // Get the variable type
            varType = String.Empty;
            auxString = tokenizedLine[2];
            auxString.Trim();
            if (auxString == String.Empty)
            {
                varType = "UNKNOWN";
            }
            else
            {
                GetEsfVarType(auxString, ref varType);
            }

            // Get the input groups (optional)
            auxString = tokenizedLine[4];
            auxString.Trim();
            if (auxString != String.Empty)
            {
                GetEsfInputGroups(auxString, ref inputGroups);
            }

            return (true);
        }

        private ImportDataModel LoadFile(string file)
        {
            ImportDataModel importDataModel = new ImportDataModelEIB(readStationName, readGroupName);

            if( ForceEISFormat.IsChecked == false )
            {
                forceEISFormat = false;
            }
            else
            {
                forceEISFormat = true;
            }
            if (CmbEisDataFormat.SelectedValue != null)                
                eisDataFormat = (EISDATAFORMAT)(((ComboBoxItem)CmbEisDataFormat.SelectedValue).Tag);
            
            file = file.ToLower();
            if (file.Contains(".esf"))
            {
                try
                {
                    using (new WaitCursor())
                    {
                        List<string[]> parsedData = new List<string[]>();
                        using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                        {
                            string line;
                            string[] row;

                            while ((line = readFile.ReadLine()) != null)
                            {
                                row = line.Split('\t');
                                parsedData.Add(row);
                            }
                        }

                       foreach (var esfFileAddress in parsedData)
                        {
                            string mainGroup = String.Empty;
                            string varDescription = String.Empty;
                            string varType = String.Empty;
                            List<string> inputGroups = new List<string>();
                            if (ParseEsfFileLine(esfFileAddress, ref mainGroup,
                                                 ref varDescription, ref varType,
                                                 ref inputGroups) == true)
                            {
                                string varName;
                                if (varDescription != String.Empty)
                                {
                                    varName = varDescription;
                                }
                                else
                                {
                                    varName = mainGroup;
                                }

                                string shownAddress = String.Empty;
                                if (mainGroup != String.Empty)
                                {
                                    shownAddress = mainGroup;
                                }
                                shownAddress += " - ";
                                if (mainGroup != String.Empty)
                                {
                                    shownAddress += mainGroup;
                                }
                                if(inputGroups.Count > 0)
                                {
                                    string auxString = String.Empty;
                                    foreach(var group in inputGroups)
                                    {
                                        if (auxString != String.Empty)
                                        {
                                            auxString += ";";
                                            auxString += group;
                                        }
                                        else
                                        {
                                            auxString = group;
                                        }
                                    }
                                    if(auxString != String.Empty)
                                    {
                                        shownAddress += " - ";
                                        shownAddress += auxString;
                                    }
                                }

                                var IVar = importDataModel.addImportData();
                                IVar.Name = varName;
                                //if (AddGroupAddressToVariableName.IsChecked == true)
                                //{                                    
                                    // add to var name group only for valid address
                                    //if (SplitMainGroup(mainGroup, out string[] mainGroupSplitted))
                                    //    IVar.Gro = string.Format("VAR_{0}_{1}_{2}_{3}", mainGroupSplitted[0], mainGroupSplitted[1], mainGroupSplitted[2], IVar.Name);
                                //}
                                ((ImportDataEIB)IVar).MainGroupName = mainGroup;
                                IVar.Address = shownAddress;
                                IVar.Select = false;
                                IVar.szType = varType;

                                // get movicon data type (here used only for tag's icon on import grid)
                                EISDATAFORMAT dataFormat = EISDATAFORMAT.EISDFBit;
                                int moviconType = -1;
                                int arrayDim = 0;
                                GetEisDataFormat(IVar.szType, IVar.Name, ref dataFormat, ref moviconType, ref arrayDim);
                                if (moviconType == -1)
                                    IVar.TagType = DataType.Boolean;
                                else
                                    IVar.TagType = (DataType)moviconType;

                                AddTreeItem(importDataModel, IVar);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message;
                    message = String.Format(Properties.Resources.ImportExceptionFileLoading, file, ex.Message);
                    MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
                    importDataModel = null;
                    //return importDataModel;
                }
            }

            //if (importDataModel == null || importDataModel.Children.Count == 0)
            //{
            //    MessageBox.Show(Properties.Resources.ImportErrorEmptyFile,
            //                    Properties.Resources.ImportMsgBoxTitle);
            //}
            //ImportTree.Model = importDataModel;

            //if (listViewSortCol != null)
            //{
            //    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
            //    ImportTree.Items.SortDescriptions.Clear();
            //}

            return importDataModel;
        }

        internal void AddTreeItem(ImportDataModel importDataModel, ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            if (parent == null)
            {
                tag.TreeLevel = 0xFF;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = -1;
                importDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = parent.Id;
                parent.Children.Add(tag);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (baseImportTree != null)
            {
                baseImportTree.LoadImportFile -= LoadFile;

                baseImportTree.Dispose();
                baseImportTree = null;
            }
        }

        #endregion
    }

    public class ImportDataEIB : ImportData
    {
        public ImportDataEIB(ImportDataModel inDataModel)
            : base(inDataModel)
        {
            dataModelEIB = inDataModel as ImportDataModelEIB;
        }
        private ImportDataModelEIB dataModelEIB { get; set; }


        public override string ToString()
        {
            return Name;
        }
        
        //private int _Id;
        //public int Id
        //{
        //    get { return _Id; }
        //    set { _Id = value; }
        //}
        //private int _parentId;
        //public int parentId
        //{
        //    get { return _parentId; }
        //    set { _parentId = value; }
        //}
        private string _Name;
        public override string Name
        {
            get
            {
                //return dataModel.getStationName() != "_" ? dataModel.getStationName() + _Name : _Name;
                return string.Format("{0}{1}{2}", dataModelEIB.getStationName(), dataModelEIB.getGroupName(_MainGroupName), _Name);
            }
            set { _Name = value; }
        }
        //private string _Address;
        //public string Address
        //{
        //    get { return _Address; }
        //    set { _Address = value; }
        //}
        //private bool _Select;
        //public bool Select
        //{
        //    get { return _Select; }
        //    set { _Select = value; }
        //}
        //private string _DynAddress;
        //public string DynAddress
        //{
        //    get { return _DynAddress; }
        //    set { _DynAddress = value; }
        //}
        //private int _TagType;
        //public int TagType
        //{
        //    get { return _TagType; }
        //    set { _TagType = value; }
        //}
        //private string _szType;
        //public string szType
        //{
        //    get { return _szType; }
        //    set { _szType = value; }
        //}

        //private uint _ArrayDimension;
        //public uint ArrayDimension
        //{
        //    get { return _ArrayDimension; }
        //    set { _ArrayDimension = value; }
        //}

        //private ImportData _Parent;
        //public ImportData Parent
        //{
        //    get { return _Parent; }
        //    set { _Parent = value; }
        //}
        //private uint _TreeLevel;
        //public uint TreeLevel
        //{
        //    get { return _TreeLevel; }
        //    set { _TreeLevel = value; }
        //}
        
        private string _MainGroupName;
        public string MainGroupName
        {
            get { return _MainGroupName; }
            set { _MainGroupName = value; }
        }

        //public string TreeName
        //{
        //    get { return getTreeName(); }
        //}

        //private string getTreeName()
        //{
        //    string treeName = Name;
        //    if (Parent != null)
        //    {
        //        treeName = Parent.getTreeName() + "." + treeName;
        //    }
        //    return treeName;
        //}
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataEIB els7 = el as ImportDataEIB;
                if (els7 != null)
                {
                    els7.Dispose();
                }
            }
            Children.Clear();
            //Children = null;
        }
    };

    public class ImportDataModelEIB : ImportDataModel, IDisposable
    {
        public GetGroupName getGroupName { get; private set; }

        public ImportDataModelEIB(GetStationName inGetStationName, GetGroupName inGetGroupName)
            : base(inGetStationName)
        {
            Children = new ObservableCollection<ImportData>();            
            getGroupName = inGetGroupName;
        }

        public override ImportData addImportData()
        {
            ImportDataEIB importData = new ImportDataEIB(this);
            return importData;
        }
        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataEIB els7 = el as ImportDataEIB;
                if (els7 != null)
                {
                    els7.Dispose();
                }
            }
            Children.Clear();
            Children = null;
        }
        #endregion
        //public System.Collections.IEnumerable GetChildren(object parent)
        //{
        //    if (parent == null)
        //        return Children;
        //    return (parent as ImportData).Children;
        //}

        //public bool HasChildren(object parent)
        //{
        //    return (parent as ImportData).Children.Count > 0;
        //}

        //public ImportData addImportData()
        //{
        //    ImportData importData = new ImportData(this);
        //    return importData;
        //}
    }
}
