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
    public class AddGate : GateData
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable
#endif
    {
#if !WINDOWS_UWP && !NET_STANDARD
#region ICloneable Members

        public override object Clone()
        {
            var ret = base.Clone() as AddGate; 
            if (Tag != null)
            {
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.Tag = tagxml.FromXml<OPCUAEntityReference>();
            }

            if (AddValueTag != null)
            {
                var tagxml = AddValueTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.AddValueTag = tagxml.FromXml<OPCUAEntityReference>();
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

            var ret = links[0].Value;
            return ret;
        }

        public override void Terminate()
        {
            _runtimeTag = null;
            _runtimeAddValueTag = null;
        }

        public override GateTypes GateType { get { return GateTypes.Output; } }

#if !WINDOWS_UWP && !NET_STANDARD
        public override bool CanDropTag() { return true; }

        public override bool DroppingTag(OPCUAEntityReference tag, String title)
        {
            Tag = tag;
            Text = title;
            return true;
        }
#endif
        public override bool ReadData()
        {
            bool ret = base.ReadData();
            var bret1 = false;
            var bret2 = false;
            if (_runtimeTag != null && _runtimeTag.MonitoredItemViewModel != null &&
                _runtimeTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeTag.MonitoredItemViewModel.DataValue.StatusCode))
                    bret1 = true;
                else
                    return false;
            }

            if (_runtimeAddValueTag != null && _runtimeAddValueTag.MonitoredItemViewModel != null &&
                _runtimeAddValueTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeAddValueTag.MonitoredItemViewModel.DataValue.StatusCode))
                    bret2 = true;
                else
                    return false;
            }

            return ret && bret1 && (bret2 || _runtimeAddValueTag == null);
        }

        public override void WriteData()
        {
            base.WriteData();

            var addValue = AddValue;
            if (_runtimeAddValueTag != null && _runtimeAddValueTag.MonitoredItemViewModel != null &&
                _runtimeAddValueTag.MonitoredItemViewModel.DataValue != null)
            {
                try
                {
                    try
                    {
                        addValue = Convert.ToDouble(_runtimeAddValueTag.MonitoredItemViewModel.DataValue.Value);
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format(Properties.Resources.WriteVariableException, _runtimeAddValueTag.HumanReadable));
                }
            }

            if (_runtimeTag != null && _runtimeTag.MonitoredItemViewModel != null &&
                _runtimeTag.MonitoredItemViewModel.DataValue != null)
            {
                try
                {
                    if (Value)
                    {
                        try
                        {
                            var sCompare = Convert.ToDouble(_runtimeTag.MonitoredItemViewModel.DataValue.Value) + addValue;
                            _runtimeTag.MonitoredItemViewModel.WriteValue(sCompare);
                        }
                        catch
                        {
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(String.Format(Properties.Resources.WriteVariableException, _runtimeTag.HumanReadable));
                }
            }
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

            if (_runtimeAddValueTag != null)
                ret.Add(_runtimeAddValueTag);
            else if (AddValueTag != null)
            {
                var tagxml = AddValueTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeAddValueTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeAddValueTag != null)
                    ret.Add(_runtimeAddValueTag);
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

                if (list.Count >= 2)
                {
                    AddValueTag = list[1];
                    tagxml = AddValueTag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeAddValueTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
        }
#endif
        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Add";
#else
                return Properties.Settings.Default.AddGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/MathLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new AddGate() { Key = "Add", Text = "Add", GateDataTemplate = "Add", Category = "Add" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.MathCategory; }
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

        OPCUAEntityReference _runtimeAddValueTag;
        OPCUAEntityReference _AddValueTag;
        public OPCUAEntityReference AddValueTag
        {
            get { return _AddValueTag; }
            set
            {
                var old = _AddValueTag;
                if (old != value)
                {
                    _AddValueTag = value;
                    RaisePropertyChanged("AddValueTag", old, value);
                }
            }
        }


        public Double AddValue
        {
            get { return _AddValue; }
            set
            {
                var old = _AddValue;
                if (old != value)
                {
                    _AddValue = value;
                    RaisePropertyChanged("AddValue", old, value);
                }
            }
        }
        private Double _AddValue = 1.0;

#endregion

#region Overrides
// support standard reading/writing via Linq for XML
#if !WINDOWS_UWP && !NET_STANDARD
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("AddValue", this.AddValue, 1.0));
            if (this.Tag != null)
                e.Add(XHelper.Attribute("Tag", this.Tag.ToXml(), ""));
            if (this.AddValueTag != null)
                e.Add(XHelper.Attribute("AddValueTag", this.AddValueTag.ToXml(), ""));
            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.AddValue = XHelper.Read("AddValue", e, 1.0);
            var tagxml = XHelper.Read("Tag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                Tag = tagxml.FromXml<OPCUAEntityReference>();
            tagxml = XHelper.Read("AddValueTag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                AddValueTag = tagxml.FromXml<OPCUAEntityReference>();
        }
        #endregion
    }
}
