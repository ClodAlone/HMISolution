using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using OPCUAViewModel;
using System.Windows;
using DocumentManager.ComponentService;

namespace LogicCore
{
    public enum GateTypes
    {
        Input,
        Output,
        Generic
    }

#if !WINDOWS_UWP
    [Serializable]
#endif
    public abstract class GateData : GraphLinksModelNodeData<String>
    {
        static GateData()
        {
            GateData.LoadGateDataTypes();
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public static readonly String NodeTemplateDictionary = "NodeTemplateDictionary";
        public static readonly String NodeCategoryPath = "GateDataTemplate";
#endif

        #region Command Type collection

        static List<Type> gateDataTypes = new List<Type>();
        static Dictionary<String, Type> gateDataTypesMap = new Dictionary<string, Type>();
        static List<String> gateDataTypeNames = new List<String>();

        public static GateData CreateFrom(String type)
        {
            if (!gateDataTypesMap.ContainsKey(type))
                return null;
            return Activator.CreateInstance(gateDataTypesMap[type]) as GateData;
        }

        public static Type GetGateDataType(String type)
        {
            if (!gateDataTypesMap.ContainsKey(type))
                return null;
            return gateDataTypesMap[type];
        }

        public static List<String> LoadGateDataTypes()
        {
            if (gateDataTypeNames.Count > 0)
                return gateDataTypeNames;

            var ret = new List<string>();

#if WINDOWS_UWP
            var list = LoadGateDataTypes("LogicCommon");
            foreach (var i in list)
                ret.Add(i);
            ret.Add("GateData");
#else
            var curAssembly = Assembly.GetExecutingAssembly();
            var basefolder = String.Format("{0}{2}{1}", Path.GetDirectoryName(curAssembly.Location), "LogicExtensions", Path.DirectorySeparatorChar);
            if (Directory.Exists(basefolder))
            {
                String[] listFiles = Directory.GetFiles(basefolder, "*.dll");

                Array.ForEach(listFiles, file =>
                {
                    var list = LoadGateDataTypes(file);
                    foreach (var i in list)
                        ret.Add(i);
                });
            }

            var v = LoadGateDataTypes(Assembly.GetAssembly(typeof(GateData)));
            foreach (var i in v)
                ret.Add(i);
#endif

            return ret;
        }

        public static List<String> LoadGateDataTypes(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Must not already exist
                if (gateDataTypes.Contains(type)) { continue; }

                // Must not be abstract.
                if ((typeof(GateData).IsAssignableFrom(type)) &&
#if !WINDOWS_UWP
                    (!type.IsAbstract)
#else
                    type != typeof(GateData)
#endif
                    )
                {
                    gateDataTypes.Add(type);
                    var am = Activator.CreateInstance(type) as GateData;
                    gateDataTypeNames.Add(am.Name);
                    gateDataTypesMap.Add(am.Name, type);
                }
            }

            return gateDataTypeNames;
        }

        public static List<String> LoadGateDataTypes(string assemblyPath)
        {
            // Load the assembly
#if WINDOWS_UWP
            var asName = new AssemblyName();
            asName.Name = assemblyPath;
            Assembly assembly = Assembly.Load(asName);
#else
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
#endif
            // Load transitions from the assembly
            return LoadGateDataTypes(assembly);
        }

#endregion

#region Abstracts

        public virtual void Init(IDocument doc) { }
        public virtual void Terminate() { }

        public abstract bool Execute(WireData[] links);
        public abstract String Name { get; }
        public virtual List<OPCUAEntityReference> GetTagList() { return new List<OPCUAEntityReference>(); }
#if !WINDOWS_UWP && !NET_STANDARD
        public virtual void UpdateTagList(List<OPCUAEntityReference> list) { }

        public virtual void OnDblClick() { }
#endif
        public virtual GateTypes GateType { get { return GateTypes.Generic; } }
        public virtual bool ReadData() { return true; }
        public virtual void WriteData() { }

#if !WINDOWS_UWP && !NET_STANDARD
        public virtual bool CanBeToggled() { return false; }
        public virtual bool CanDropTag() { return false; }
        public virtual bool DroppingTag(OPCUAEntityReference tag, String title) { return false; }

        public virtual DataTemplateDictionary GetDataTemplates() { return null; }
        public virtual String GetCategory() { return Properties.Resources.CommonCategory; }
        public virtual List<GateData> GetTypes() { return new List<GateData>(); }
#endif
#endregion

#region Properties
        public bool Visited { get; set; }

#if !WINDOWS_UWP && !NET_STANDARD
        public String GateDataTemplate
        {
            get
            {
                if (String.IsNullOrEmpty(_GateDataTemplate))
                    return Name;
                return _GateDataTemplate;
            }
            protected set
            {
                String old = _GateDataTemplate;
                if (old != value)
                {
                    _GateDataTemplate = value;
                    RaisePropertyChanged("GateDataTemplate", old, value);
                }
            }
        }
        private String _GateDataTemplate;
#endif
        public Boolean Value
        {
            get { return _Value; }
            set
            {
                Boolean old = _Value;
                if (old != value)
                {
                    _Value = value;
                    RaisePropertyChanged("Value", old, value);
                }
            }
        }
        private Boolean _Value;
#endregion

#region Overrides
// support standard reading/writing via Linq for XML
#if !WINDOWS_UWP && !NET_STANDARD
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("GateDataTemplate", this.GateDataTemplate, ""));

            e.Add(XHelper.Attribute("Value", this.Value, false));
            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
#if !WINDOWS_UWP && !NET_STANDARD
            this.GateDataTemplate = XHelper.Read("GateDataTemplate", e, "");
#endif
            this.Value = XHelper.Read("Value", e, false);
        }
#endregion
    }


#if !WINDOWS_UWP
    [Serializable]
#endif
    public class WireData : GraphLinksModelLinkData<String, String>
    {
        // the link color is bound to its Data's Value property and must update with it
        public Boolean Value
        {
            get { return _Value; }
            set
            {
                Boolean old = _Value;
                if (old != value)
                {
                    _Value = value;
                    RaisePropertyChanged("Value", old, value);
                }
            }
        }
        private Boolean _Value;

        // the only additional property, Value, is not meant to be persistent
        // on "wires", so we don't need to override MakeXElement and LoadFromXElement
    }
}
