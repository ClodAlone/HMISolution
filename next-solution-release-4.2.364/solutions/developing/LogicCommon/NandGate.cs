using LogicCore;
using Northwoods.GoXam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogicCommon
{
    public class NandGate : GateData
    {
        #region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            var ret = links[0].Value;
            foreach (var data in links)
                ret &= data.Value;
            return !ret;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Nand";
#else
                return Properties.Settings.Default.NandGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/NandLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new NandGate() { Key = "Nand", Text = "Nand", GateDataTemplate = "Nand", Category = "Nand" });
            ret.Add(new NandGate() { Key = "Nand", Text = "Nand", GateDataTemplate = "NandFour", Category = "NandFour" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.PortCategory; }
#endif
#endregion
    }
}
