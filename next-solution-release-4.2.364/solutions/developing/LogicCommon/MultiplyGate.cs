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
    public class MultiplyGate : GateData
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable
#endif
    {
#if !WINDOWS_UWP && !NET_STANDARD
#region ICloneable Members

        public override object Clone()
        {
            var ret = base.Clone() as MultiplyGate; 
            if (Tag != null)
            {
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.Tag = tagxml.FromXml<OPCUAEntityReference>();
            }
            if (MultiplyValueTag != null)
            {
                var tagxml = MultiplyValueTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.MultiplyValueTag = tagxml.FromXml<OPCUAEntityReference>();
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
            }

            if (_runtimeMultiplyValueTag != null && _runtimeMultiplyValueTag.MonitoredItemViewModel != null &&
                _runtimeMultiplyValueTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeMultiplyValueTag.MonitoredItemViewModel.DataValue.StatusCode))
                    bret2 = true;
            }

            return ret && bret1 && (bret2 || _runtimeMultiplyValueTag == null);
        }

        public override void WriteData()
        {
            base.WriteData();

            var multiplyValue = MultiplyValue;
            if (_runtimeMultiplyValueTag != null && _runtimeMultiplyValueTag.MonitoredItemViewModel != null &&
                _runtimeMultiplyValueTag.MonitoredItemViewModel.DataValue != null)
            {
                try
                {
                    try
                    {
                        multiplyValue = Convert.ToDouble(_runtimeMultiplyValueTag.MonitoredItemViewModel.DataValue.Value);
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format(Properties.Resources.WriteVariableException, _runtimeMultiplyValueTag.HumanReadable));
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
                            var sCompare = Convert.ToDouble(_runtimeTag.MonitoredItemViewModel.DataValue.Value) * multiplyValue;
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

            if (_runtimeMultiplyValueTag != null)
                ret.Add(_runtimeMultiplyValueTag);
            else if (MultiplyValueTag != null)
            {
                var tagxml = MultiplyValueTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeMultiplyValueTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeMultiplyValueTag != null)
                    ret.Add(_runtimeMultiplyValueTag);
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
                    MultiplyValueTag = list[1];
                    tagxml = MultiplyValueTag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeMultiplyValueTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
        }
#endif
        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Multiply";
#else
                return Properties.Settings.Default.MultiplyGateName;
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
            ret.Add(new MultiplyGate() { Key = "Multiply", Text = "Multiply", GateDataTemplate = "Multiply", Category = "Multiply" });
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

        OPCUAEntityReference _runtimeMultiplyValueTag;
        OPCUAEntityReference _MultiplyValueTag;
        public OPCUAEntityReference MultiplyValueTag
        {
            get { return _MultiplyValueTag; }
            set
            {
                var old = _MultiplyValueTag;
                if (old != value)
                {
                    _MultiplyValueTag = value;
                    RaisePropertyChanged("MultiplyValueTag", old, value);
                }
            }
        }

        public Double MultiplyValue
        {
            get { return _MultiplyValue; }
            set
            {
                var old = _MultiplyValue;
                if (old != value)
                {
                    _MultiplyValue = value;
                    RaisePropertyChanged("Multiply", old, value);
                }
            }
        }
        private Double _MultiplyValue = 1.0;

        #endregion

        #region Overrides
        // support standard reading/writing via Linq for XML
#if !WINDOWS_UWP && !NET_STANDARD
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("MultiplyValue", this.MultiplyValue, 1.0));
            if (this.Tag != null)
                e.Add(XHelper.Attribute("Tag", this.Tag.ToXml(), ""));
            if (this.MultiplyValueTag != null)
                e.Add(XHelper.Attribute("MultiplyValueTag", this.MultiplyValueTag.ToXml(), ""));
            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.MultiplyValue = XHelper.Read("MultiplyValue", e, 1.0);
            var tagxml = XHelper.Read("Tag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                Tag = tagxml.FromXml<OPCUAEntityReference>();
            tagxml = XHelper.Read("MultiplyValueTag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                MultiplyValueTag = tagxml.FromXml<OPCUAEntityReference>();
        }
        #endregion
    }
}
