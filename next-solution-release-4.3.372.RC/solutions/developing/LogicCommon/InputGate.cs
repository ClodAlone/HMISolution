using LogicCore;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Utilities;

namespace LogicCommon
{
    public class InputGate : GateData
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable
#endif
    {
#if !WINDOWS_UWP && !NET_STANDARD
#region ICloneable Members

        public override object Clone()
        {
            var ret = base.Clone() as InputGate;
            if (Tag != null)
            {
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.Tag = tagxml.FromXml<OPCUAEntityReference>();
            }

            return ret;
        }

#endregion
#endif

#region Abstracts
        public override bool Execute(WireData[] links)
        {
            return true;
        }

        public override void Terminate()
        {
            _runtimeTag = null;
        }

        public override GateTypes GateType { get { return GateTypes.Input; } }

        public override bool ReadData()
        {
            bool ret = base.ReadData();
            if (_runtimeTag != null && _runtimeTag.MonitoredItemViewModel != null &&
                _runtimeTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeTag.MonitoredItemViewModel.DataValue.StatusCode))
                {
                    try
                    {
                        Value = Convert.ToBoolean(_runtimeTag.MonitoredItemViewModel.Value);
                        ret = true;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            var v = Convert.ToInt32(_runtimeTag.MonitoredItemViewModel.Value);
                            Value = v != 0;
                            ret = true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                    return false;
            }

            return ret;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public override bool CanBeToggled() { return true; }

        public override bool CanDropTag() { return true; }

        public override bool DroppingTag(OPCUAEntityReference tag, String title)
        {
            Tag = tag;
            Text = title;
            return true;
        }
#endif
        public override List<OPCUAEntityReference> GetTagList()
        {
            var ret = base.GetTagList();
            if (_runtimeTag != null)
                ret.Add(_runtimeTag);
            else if (Tag != null)
            {
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeTag != null)
                    ret.Add(_runtimeTag);
            }
            return ret;
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override void UpdateTagList(List<OPCUAEntityReference> list)
        {
            if (list == null)
                return;
            if (list.Count >= 1)
            {
                Tag = list[0];
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeTag = tagxml.FromXml<OPCUAEntityReference>();
            }
        }
#endif
        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Input";
#else
                return Properties.Settings.Default.InputGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/InputLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new InputGate() { Key = "Input", Text = "Input", GateDataTemplate = "Input", Category = "Input" });
            return ret;
        }
#endif
        #endregion

        #region Properties

        OPCUAEntityReference _runtimeTag;
        OPCUAEntityReference _Tag;
        public OPCUAEntityReference Tag
        {
            get { return _Tag; }
            set
            {
                var old = _Tag;
                if (old != value)
                {
                    _Tag = value;
                    RaisePropertyChanged("Tag", old, value);
                }
            }
        }
#endregion

#region Overrides
// support standard reading/writing via Linq for XML
#if !WINDOWS_UWP && !NET_STANDARD
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            if (this.Tag != null)
                e.Add(XHelper.Attribute("Tag", this.Tag.ToXml(), ""));
            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            var tagxml = XHelper.Read("Tag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                Tag = tagxml.FromXml<OPCUAEntityReference>();
        }

#endregion
       
    }
}
