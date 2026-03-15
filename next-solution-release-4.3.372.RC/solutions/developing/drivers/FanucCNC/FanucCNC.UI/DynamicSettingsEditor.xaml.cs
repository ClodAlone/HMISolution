using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace FanucCNC.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {
        private class Category
        {
            public FanucCNCProtocol.FunctionCategory Code { get; set; }
            public string Description { get; set; }

            public Category(FanucCNCProtocol.FunctionCategory code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        public class Function
        {
            public FanucCNCProtocol.FunctionCategory Category { get; set; }
            public FanucCNCProtocol.FunctionCode Code { get; set; }
            public string Description { get; set; }

            public Function(FanucCNCProtocol.FunctionCategory category, FanucCNCProtocol.FunctionCode code, string description)
            {
                Category = category;
                Code = code;
                Description = description;
            }
        }

        const string FUNCTION_CNT = "FC";
        const int FUNCTION_CNT_INDEX = 1;

        bool bLoaded = false;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        FanucCNCDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        FanucCNCDynTagSettings thisTagSettings;

        private Dictionary<string, FanucCNCStationSettings> stationSettingList;
        private Dictionary<string, FanucCNCChannelSettings> channelSettingList;

        DriverCodeBase.UI.Controls.BaseDynamicSettings baseDyn;

        FanucCNCProtocol.MachineSeries machineSerie;
        //short cncPath;

        public delegate void InterfaceDataChanged(FanucCNCProtocol.MachineSeries machineSerie);//, short cncPath);

        public InterfaceDataChanged interfaceDataChanged;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {                
                if (bLoaded)
                    return;

                bLoaded = true;

                //var baseDyn = new DriverCodeBae.Controls.BaseDynamicSettings();
                baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();

                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new FanucCNCDynTagSettings() { IsMethod = thisTag.IsMethod };//driver specific class
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                    LoadbaseDyn = true;
                }

                if (!thisTag.IsMethod)
                {
                    baseDyn.CmbMethod.Visibility = System.Windows.Visibility.Collapsed;
                    baseDyn.TMethod.Visibility = System.Windows.Visibility.Collapsed;
                    thisTagSettings.MethodID = -1;
                }

                DataContext = null;
                DataContext = thisTagSettings;
                baseDyn.DataContext = DataContext;
                if (LoadbaseDyn)
                    MainStack.Children.Add(baseDyn);

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    return;
                }

                ufw = new UnitOfWork(idl);
                DriverCodeBase.CommunicationDriver.UpdateDriverSchema(ufw);
                try
                {
                    //driver specific class
                    configuration = (from tag in new XPQuery<FanucCNCDriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch (InvalidOperationException ex)
                {
                    //driver specific class
                    configuration = new FanucCNCDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                    stationSettingList = new Dictionary<string, FanucCNCStationSettings>();
                    foreach (FanucCNCStationSettings settings in configuration.StationSettings)
                    {
                        if (!stationSettingList.ContainsKey(settings.Name))
                            stationSettingList.Add(settings.Name, settings);
                    }
                    channelSettingList = new Dictionary<string, FanucCNCChannelSettings>();
                    foreach (FanucCNCChannelSettings settings in configuration.ChannelSettings)
                    {
                        if (!channelSettingList.ContainsKey(settings.Name))
                            channelSettingList.Add(settings.Name, settings);
                    }
                }

                // disable default driver's unsupported feature
                baseDyn.TElemNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytesText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytes.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWordsText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWords.Visibility = System.Windows.Visibility.Collapsed;
                
                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                if (string.IsNullOrEmpty(thisTagSettings.StationName))
                {
                    if (baseDyn.CmbStation.HasItems)
                        thisTagSettings.StationName = ((FanucCNCStationSettings)baseDyn.CmbStation.Items[0]).Name;
                }

                // retrive machine specific from channel/statiion settings
                GetSelecedStationParameters(thisTagSettings.StationName, out machineSerie); //, out cncPath);                

                baseDyn.CmbStation.SelectionChanged += new SelectionChangedEventHandler(CmbStation_SelectionChanged);

                List<Category> cat = new List<Category>();
                cat.Add(new Category(FanucCNCProtocol.FunctionCategory.Pmc, "Pmc"));
                cat.Add(new Category(FanucCNCProtocol.FunctionCategory.Cnc, "Cnc"));
                cat.Add(new Category(FanucCNCProtocol.FunctionCategory.FileManagement, "File management"));                
                cmbCategory.ItemsSource = cat;

                if (thisTagSettings.FunctionCode == FanucCNCProtocol.FunctionCode.Unknown)
                    thisTagSettings.FunctionCode = GetFirstFunctionCodeOfCategory(FanucCNCProtocol.FunctionCategory.Pmc);

                if (thisTagSettings.FunctionCode != FanucCNCProtocol.FunctionCode.Unknown)
                    cmbCategory.SelectedValue = GetCategoryByFunctionCode(thisTagSettings.FunctionCode);                
            };
            IsVisibleChanged += (o, e) =>
            {
                bVisibleOnce |= (bool)e.NewValue;
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded && bVisibleOnce)
                {
                    // during reaload operation (cause by changing driver's tab, some trouble occours when try to add/remove control by FunctionCode; disable interface reload
                    //bLoaded = false;
                    var s = DataContext as FanucCNCDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private bool IsFunctionCodeIntoCategory(FanucCNCProtocol.FunctionCategory category, FanucCNCProtocol.FunctionCode functionCode)
        {            
            return getListFunction(category).ContainsKey(functionCode);
        }

        private FanucCNCProtocol.FunctionCode GetFirstFunctionCodeOfCategory(FanucCNCProtocol.FunctionCategory category)
        {
            return getListFunction(category).Values.ToList()[0].Code;
        }

        private FanucCNCProtocol.FunctionCategory GetCategoryByFunctionCode(FanucCNCProtocol.FunctionCode functionCode)
        {
            if (getListFunction(FanucCNCProtocol.FunctionCategory.Pmc).ContainsKey(functionCode))
                return FanucCNCProtocol.FunctionCategory.Pmc;

            if (getListFunction(FanucCNCProtocol.FunctionCategory.Cnc).ContainsKey(functionCode))
                return FanucCNCProtocol.FunctionCategory.Cnc;

            if (getListFunction(FanucCNCProtocol.FunctionCategory.FileManagement).ContainsKey(functionCode))
                return FanucCNCProtocol.FunctionCategory.FileManagement;

            return FanucCNCProtocol.FunctionCategory.Pmc;
        }

        private List<Function> GetListFunction(FanucCNCProtocol.FunctionCategory category)
        {            
            return getListFunction(category).Values.ToList();
        }

        private Dictionary<FanucCNCProtocol.FunctionCode, Function> getListFunction(FanucCNCProtocol.FunctionCategory category)
        {
            Dictionary<FanucCNCProtocol.FunctionCode, Function> lst = new Dictionary<FanucCNCProtocol.FunctionCode, Function>();

            switch (category)
            {
                case FanucCNCProtocol.FunctionCategory.Pmc:
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng, new Function(FanucCNCProtocol.FunctionCategory.Pmc, FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng, "Read/Write PMC data (pmc_rd/wrpmcrng)"));
                    break;
                case FanucCNCProtocol.FunctionCategory.Cnc:
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_actf, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_actf, "Read actual axis feedrate(F) - (cnc_actf)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_absolute, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_absolute, "Read absolute axis position - (cnc_absolute)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_rdaxisdata, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_rdaxisdata, "Read various data relating servo axis or spindle axis - (cnc_rdaxisdata)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_rdtofsr_cnc_wrtofsr, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_rdtofsr_cnc_wrtofsr, "Read/Write tool offset value (cnc_rd/wrtofsr)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_rdparam_cnc_wrparam, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_rdparam_cnc_wrparam, "Read/Write parameter (cnc_rd/wrparam)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_statinfo, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_statinfo, "Read CNC status information (cnc_statinfo)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_rdsvmeter, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_rdsvmeter, "Read servo load meter (cnc_rdsvmeter)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_rdzofs_cnc_wrzofs, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_rdzofs_cnc_wrzofs, "Read/Write work zero offset value - (cnc_rd/wrzofs)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_rdspeed, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_rdspeed, "Read speed information - (cnc_rdspeed)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_rdalmmsg2, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_rdalmmsg2, "Read alarm message - (cnc_rdalmmsg2)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_rdopmsg3, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_rdopmsg3, "Read operator's message - (cnc_rdopmsg3)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdactpt, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdactpt, "Read the actual execution pointer - (cnc_pdf_rdactpt)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_getpath_cnc_setpath, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_getpath_cnc_setpath, "Read/Write path number (cnc_get/setpath)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdmain, new Function(FanucCNCProtocol.FunctionCategory.Cnc, FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdmain, "Read main program - (cnc_pdf_rdmain)"));
                    break;
                case FanucCNCProtocol.FunctionCategory.FileManagement:
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_pdf_slctmain, new Function(FanucCNCProtocol.FunctionCategory.FileManagement, FanucCNCProtocol.FunctionCode.Func_cnc_pdf_slctmain, "Select main program - (cnc_pdf_slctmain)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_cnc_pdf_del, new Function(FanucCNCProtocol.FunctionCategory.FileManagement, FanucCNCProtocol.FunctionCode.Func_cnc_pdf_del, "Delete folder or file - (cnc_pdf_del)"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_Custom_DownloadProgramToCnc, new Function(FanucCNCProtocol.FunctionCategory.FileManagement, FanucCNCProtocol.FunctionCode.Func_Custom_DownloadProgramToCnc, "Download program to Cnc"));
                    lst.Add(FanucCNCProtocol.FunctionCode.Func_Custom_UploadProgramFromCnc, new Function(FanucCNCProtocol.FunctionCategory.FileManagement, FanucCNCProtocol.FunctionCode.Func_Custom_UploadProgramFromCnc, "Upload program from Cnc"));
                    break;
            }

            return lst;
        }


        private void GetSelecedStationParameters(object selectedValue, out FanucCNCProtocol.MachineSeries machineSerie)//, out short cncPath)
        {
            machineSerie = FanucCNCProtocol.MachineSeries.Serie0iB;
            //cncPath = 0;            
            if (selectedValue != null && !string.IsNullOrWhiteSpace(selectedValue.ToString()))
            {
                string station = selectedValue.ToString();
                if (stationSettingList.ContainsKey(station))
                {
                    FanucCNCStationSettings st = stationSettingList[station];
                    if (channelSettingList.ContainsKey(st.Channel))
                    {
                        FanucCNCChannelSettings ch = channelSettingList[st.Channel];
                        //if (thisTagSettings.FunctionSettings != null)
                        //{
                            machineSerie = ch.MachineSerie;
                            //cncPath = st.CNCPath;
                        //}
                    }
                }
            }
        }
        
        private void CmbStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedValue != null)
            {
                GetSelecedStationParameters(((ComboBox)sender).SelectedValue, out FanucCNCProtocol.MachineSeries machineSerie); //, out short cncPath);

                if (interfaceDataChanged != null)
                    interfaceDataChanged(machineSerie);//, cncPath);
            }
        }


        /// <summary>
        /// Remove (if present) 'custom control' associated to selected function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveLoadedFunctionCNT()
        {                        
            if (MainStack.Children.Count > FUNCTION_CNT_INDEX)
            {
                UserControl ch = MainStack.Children[FUNCTION_CNT_INDEX] as UserControl;
                if (ch != null)
                {
                    if (ch.Tag != null && Convert.ToString(ch.Tag) == FUNCTION_CNT)
                    {
                        MainStack.Children.RemoveAt(FUNCTION_CNT_INDEX);                        
                    }
                }
            }
        }

        /// <summary>
        /// Add 'custom control' (with bind data) associated to selected function
        /// </summary>
        /// <param name="cnt"></param>
        private void AddFunctionCNT(UserControl cnt)
        {
            if (cnt == null)
                cnt = new DynamicSettingsControl_NoParameters();

            // mark the new control as Function control; use this information later to remove from list
            cnt.Tag = FUNCTION_CNT;

            if (cnt is IDynamicSettingsChildrenControlRefresh)
                interfaceDataChanged = ((IDynamicSettingsChildrenControlRefresh)cnt).interfaceDataChanged;

            // set
            thisTagSettings.FunctionSettings.SetChannelStationInfo(machineSerie);//, cncPath);
            // force to validate base parameters (link type, array dimension, ecc) when load specific control
            thisTagSettings.ForceFunctionSettingsBaseClassValidation();
            cnt.DataContext = thisTagSettings.FunctionSettings;
            cnt.Width = this.Width;
            MainStack.Children.Insert(FUNCTION_CNT_INDEX, cnt);
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // some category selected ?
            if (cmbCategory.SelectedIndex != -1)
            {
                // get selected category
                Category cat = cmbCategory.Items[cmbCategory.SelectedIndex] as Category;
                if (cat != null)
                {                    
                    // fill function's list with category
                    cmbFunction.ItemsSource = GetListFunction(cat.Code);
                    // no selected function code ?
                    if (cmbFunction.SelectedIndex == -1)
                    {
                        // get 1st from selected category
                        cmbFunction.SelectedValue = GetFirstFunctionCodeOfCategory(cat.Code);
                    }
                    else
                    {
                        FanucCNCProtocol.FunctionCode fc = ((Function)cmbFunction.Items[cmbFunction.SelectedIndex]).Code;
                        // if current function code is not compatible with category, set firest element of category as new value
                        if (!IsFunctionCodeIntoCategory(cat.Code, fc))
                            cmbFunction.SelectedValue = GetFirstFunctionCodeOfCategory(cat.Code);
                    }
                }
            }
        }

        private void cmbFunction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFunction.SelectedIndex == -1)
                return;

            UserControl cnt = null;

            // remove (if present) 'custom control' associated tp selected function
            RemoveLoadedFunctionCNT();

            if (thisTagSettings.FunctionSettings != null)
                thisTagSettings.FunctionSettings.MachineModel = machineSerie;

            switch (((Function)cmbFunction.Items[cmbFunction.SelectedIndex]).Code)
            {
                case FanucCNCProtocol.FunctionCode.Func_cnc_actf:
                    {                        
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_actf) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_actf(thisTagSettings, string.Empty);
                        // no user control
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_absolute:
                    {
                        cnt = new DynamicSettingsControl_cnc_absolute();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_absolute) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_absolute(thisTagSettings, string.Empty);
                        // no user control
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdaxisdata:
                    {
                        cnt = new DynamicSettingsControl_cnc_rdaxisdata();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_rdaxisdata) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_rdaxisdata(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdzofs_cnc_wrzofs:
                    {
                        cnt = new DynamicSettingsControl_cnc_rdzofs_cnc_wrzofs();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdspeed:
                    {
                        cnt = new DynamicSettingsControl_cnc_rdspeed();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_rdspeed) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_rdspeed(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdopmsg3:
                    {
                        cnt = new DynamicSettingsControl_cnc_rdopmsg3();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_rdopmsg3) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_rdopmsg3(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdactpt:
                    {
                        cnt = new DynamicSettingsControl_cnc_pdf_rdactpt();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_pdf_rdactpt) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_pdf_rdactpt(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdmain:
                    {
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_pdf_rdmain) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_pdf_rdmain(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdalmmsg2:
                    {
                        cnt = new DynamicSettingsControl_cnc_rdalmmsg2();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_rdalmmsg2) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_rdalmmsg2(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdsvmeter:
                    {
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_rdsvmeter) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_rdsvmeter(thisTagSettings, string.Empty);
                        // no user control
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng:
                    {
                        cnt = new DynamicSettingsControl_pmc_rdpmcrng_pmc_wrpmcrng();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng) == null)    
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng(thisTagSettings, string.Empty);                        
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_statinfo:
                    {
                        cnt = new DynamicSettingsControl_cnc_statinfo();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_statinfo) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_statinfo(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_getpath_cnc_setpath:
                    {                        
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_getpath_cnc_setpath) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_getpath_cnc_setpath(thisTagSettings, string.Empty);
                        // no user control
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdparam_cnc_wrparam:
                    {
                        cnt = new DynamicSettingsControl_cnc_rdparam_cnc_wrparam();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_rdparam_cnc_wrparam) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_rdparam_cnc_wrparam(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdtofsr_cnc_wrtofsr:
                    {
                        cnt = new DynamicSettingsControl_cnc_rdtofsr_cnc_wrtofsr();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_del:
                    cnt = new DynamicSettingsControl_cnc_pdf_del();
                    if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_pdf_del) == null)
                        thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_pdf_del(thisTagSettings, string.Empty);                    
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_slctmain:
                    cnt = new DynamicSettingsControl_cnc_pdf_slctmain();
                    if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_cnc_pdf_slctmain) == null)
                        thisTagSettings.FunctionSettings = new FanucCNCDynTag_cnc_pdf_slctmain(thisTagSettings, string.Empty);
                    break;


                case FanucCNCProtocol.FunctionCode.Func_Custom_DownloadProgramToCnc:
                    {
                        cnt = new DynamicSettingsControl_Custom_DownloadProgramToCnc();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_Custom_DownloadProgramToCnc) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_Custom_DownloadProgramToCnc(thisTagSettings, string.Empty);
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_Custom_UploadProgramFromCnc:
                    {
                        cnt = new DynamicSettingsControl_Custom_UploadProgramFromCnc();
                        if ((thisTagSettings.FunctionSettings as FanucCNCDynTag_Custom_UploadProgramFromCnc) == null)
                            thisTagSettings.FunctionSettings = new FanucCNCDynTag_Custom_UploadProgramFromCnc(thisTagSettings, string.Empty);
                    }
                    break;
            }
            
            AddFunctionCNT(cnt);

            if (((FanucCNCDynTag_BaseFunction)thisTagSettings.FunctionSettings).CNCPathSupported)
            {
                // reset control's vlaues
                lblCNCPath.Visibility = System.Windows.Visibility.Visible;
                txtCNCPath.Visibility = System.Windows.Visibility.Visible;
            } else
            {                
                // reset control's vlaues
                lblCNCPath.Visibility = System.Windows.Visibility.Collapsed;
                txtCNCPath.Visibility = System.Windows.Visibility.Collapsed;
                thisTagSettings.CNCPath = 0;
            }

            if (((FanucCNCDynTag_BaseFunction)thisTagSettings.FunctionSettings).OffsetVariableSupported) {
                baseDyn.SetOffsetVariableVisible(true);
            } else {
                baseDyn.SetOffsetVariableVisible(false);
                // reset control's vlaues
                thisTagSettings.OffsetVariableId = string.Empty;
                thisTagSettings.OffsetVariableName = string.Empty;
            }
        }

        #region Property
        private string _Connection;
        public string Connection
        {
            get { return _Connection; }
            set
            {
                _Connection = value;
            }
        }
        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
        }
        #endregion

        #region Offset variable management
        private static class NodeIdHelper
        {
            public static bool TryParse(string text, out Opc.Ua.NodeId nodeid)
            {
                try
                {
                    nodeid = Opc.Ua.NodeId.Parse(text);
                }
                catch
                {
                    nodeid = Opc.Ua.NodeId.Null;
                    return false;
                }

                return true;
            }
        }

        //private void btnOffsetVariableCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var nDC = DataContext as FanucCNCDynTagSettings;
        //    nDC.OffsetVariableName = null;
        //    nDC.OffsetVariableId = null;
        //}

        //private void btnOffsetVariableBrowse_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var nDC = DataContext as FanucCNCDynTagSettings;
        //    Opc.Ua.NodeId nodeId;
        //    if (!NodeIdHelper.TryParse(nDC.OffsetVariableId, out nodeId))
        //        nodeId = Opc.Ua.NodeId.Null;
        //    var tagEntityReference = new UFUAModel.TagEntityReference(Guid.Empty, nDC.OffsetVariableName, nodeId);
        //    var tag = tagEntityReference.Edit(this.FindParent<Window>());
        //    if (tag != null)
        //    {
        //        nDC.OffsetVariableId = tag.NodeId.ToString();
        //        nDC.OffsetVariableName = tag.ToString();
        //    }
        //}
        #endregion
    }
}
