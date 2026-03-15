using LogicCore;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
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
    public enum CompareTypes
    {
        Equal,
        Greater,
        Lower,
        GreaterAndEqual,
        LowerAndEqual
    }

    public class CompareGate : GateData
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable
#endif
    {
#if !WINDOWS_UWP && !NET_STANDARD
#region ICloneable Members

        public override object Clone()
        {
            var ret = base.Clone() as CompareGate;
            if (Tag != null)
            {
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.Tag = tagxml.FromXml<OPCUAEntityReference>();
            }

            if (CompareTag != null)
            {
                var tagxml = CompareTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.CompareTag = tagxml.FromXml<OPCUAEntityReference>();
            }

            return ret;
        }

#endregion
#endif

#region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            if (!links[0].Value)
                return false;
            var ret = false;
            if (_runtimeTag != null && _runtimeTag.MonitoredItemViewModel != null &&
                _runtimeTag.MonitoredItemViewModel.DataValue != null)
            {
                String s1 = String.Empty, s2 = String.Empty;
                bool bs1 = false, bs2 = false;
                double value1 = 0;
                double value2 = 0;
                try
                {
                    if (_runtimeTag.MonitoredItemViewModel.DataValue.Value is String)
                    {
                        s1 = _runtimeTag.MonitoredItemViewModel.DataValue.Value as String;
                        bs1 = true;
                    }
                    else
                        value1 = Convert.ToDouble(_runtimeTag.MonitoredItemViewModel.DataValue.Value);
                }
                catch
                {

                }

                if (_runtimeCompareTag != null && _runtimeCompareTag.MonitoredItemViewModel != null &&
                    _runtimeCompareTag.MonitoredItemViewModel.DataValue != null)
                {
                    try
                    {
                        if (_runtimeCompareTag.MonitoredItemViewModel.DataValue.Value is String)
                        {
                            s2 = _runtimeCompareTag.MonitoredItemViewModel.DataValue.Value as String;
                            bs2 = true;
                        }
                        else
                            value2 = Convert.ToDouble(_runtimeCompareTag.MonitoredItemViewModel.DataValue.Value);
                    }
                    catch
                    {

                    }
                }
                else
                    value2 = CompareValue;

                if (bs1 && bs2)
                {
                    switch (CompareType)
                    {
                        case CompareTypes.Equal: ret = s1 == s2; break;
                        case CompareTypes.Greater: ret = s1.Length > s2.Length; break;
                        case CompareTypes.Lower: ret = s1.Length < s2.Length; break;
                        case CompareTypes.GreaterAndEqual: ret = s1.Length >= s2.Length; break;
                        case CompareTypes.LowerAndEqual: ret = s1.Length <= s2.Length; break;
                    }
                }
                else
                {
                    switch (CompareType)
                    {
                        case CompareTypes.Equal: ret = value1 == value2; break;
                        case CompareTypes.Greater: ret = value1 > value2; break;
                        case CompareTypes.Lower: ret = value1 < value2; break;
                        case CompareTypes.GreaterAndEqual: ret = value1 >= value2; break;
                        case CompareTypes.LowerAndEqual: ret = value1 <= value2; break;
                    }
                }
            }

            return ret;
        }

        public override void Terminate()
        {
            _runtimeTag = null;
            _runtimeCompareTag = null;
        }

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

            if (_runtimeCompareTag != null)
                ret.Add(_runtimeCompareTag);
            else if (CompareTag != null)
            {
                var tagxml = CompareTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeCompareTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeCompareTag != null)
                    ret.Add(_runtimeCompareTag);
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
                if (Tag != null)
                {
                    Tag = list[0];
                    var tagxml = Tag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
            if (list.Count >= 2)
            {
                if (CompareTag != null)
                {
                    CompareTag = list[1];
                    var tagxml = CompareTag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeCompareTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
        }
#endif
        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Compare";
#else
                return Properties.Settings.Default.CompareGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/CompareLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new CompareGate() { Key = "Compare", Text = "Compare", GateDataTemplate = "Compare", Category = "Compare" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.CompareCategory; }
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

        OPCUAEntityReference _runtimeCompareTag;
        OPCUAEntityReference _CompareTag;
        public OPCUAEntityReference CompareTag
        {
            get { return _CompareTag; }
            set
            {
                var old = _CompareTag;
                if (old != value)
                {
                    _CompareTag = value;
                    RaisePropertyChanged("CompareTag", old, value);
                }
            }
        }

        public double CompareValue
        {
            get { return _CompareValue; }
            set
            {
                var old = _CompareValue;
                if (old != value)
                {
                    _CompareValue = value;
                    RaisePropertyChanged("CompareValue", old, value);
                }
            }
        }
        private double _CompareValue;
        public CompareTypes CompareType
        {
            get { return _CompareType; }
            set
            {
                var old = _CompareType;
                if (old != value)
                {
                    _CompareType = value;
                    RaisePropertyChanged("CompareType", old, value);
                }
            }
        }
        private CompareTypes _CompareType;

#endregion

#region Overrides

#if !WINDOWS_UWP && !NET_STANDARD

        public override bool CanDropTag() { return true; }

        public override bool DroppingTag(OPCUAEntityReference tag, String title)
        {
            Tag = tag;
            Text = title;
            return true;
        }

        // support standard reading/writing via Linq for XML
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.AttributeEnum<CompareTypes>("CompareType", this.CompareType, CompareTypes.Equal));
            e.Add(XHelper.Attribute("CompareValue", this.CompareValue, 0));
            if (this.Tag != null)
                e.Add(XHelper.Attribute("Tag", this.Tag.ToXml(), ""));
            if (this.CompareTag != null)
                e.Add(XHelper.Attribute("CompareTag", this.CompareTag.ToXml(), ""));
            return e;
        }
#endif

        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.CompareType = XHelper.ReadEnum<CompareTypes>("CompareType", e, CompareTypes.Equal);
            this.CompareValue = XHelper.Read("CompareValue", e, 0);
            var tagxml = XHelper.Read("Tag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                Tag = tagxml.FromXml<OPCUAEntityReference>();
            tagxml = XHelper.Read("CompareTag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                CompareTag = tagxml.FromXml<OPCUAEntityReference>();
        }
#endregion
    }
}
