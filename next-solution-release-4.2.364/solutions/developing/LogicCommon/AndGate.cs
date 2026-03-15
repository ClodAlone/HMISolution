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
    public class AndGate : GateData
    {
        #region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            var ret = links[0].Value;
            foreach (var data in links)
                ret &= data.Value;
            return ret;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "And";
#else
                return Properties.Settings.Default.AndGateName;
#endif
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/AndLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new AndGate() { Key = "And", Text = "And", GateDataTemplate = "And", Category = "And" });
            ret.Add(new AndGate() { Key = "And", Text = "And", GateDataTemplate = "AndFour", Category = "AndFour" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.PortCategory; }
#endif
        #endregion
    }
}
