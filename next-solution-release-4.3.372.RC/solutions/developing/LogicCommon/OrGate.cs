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
    public class OrGate : GateData
    {
        #region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            var ret = links[0].Value;
            foreach (var data in links)
                ret |= data.Value;
            return ret;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Or";
#else
                return Properties.Settings.Default.OrGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/OrLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new OrGate() { Key = "Or", Text = "Or", GateDataTemplate = "Or", Category = "Or" });
            ret.Add(new OrGate() { Key = "Or", Text = "Or", GateDataTemplate = "OrFour", Category = "OrFour" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.PortCategory; }
#endif
#endregion
    }
}
