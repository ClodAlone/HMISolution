using DocumentManager.ComponentService;
using Jint;
using LogicCore;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using UFProjectManager.ScriptHelpers;
using Utilities;

namespace LogicCommon
{
    public class ScriptGate : GateData
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable
#endif
    {
        #region members
        Engine engine;
        Servers servers;

        String oldText;
        #endregion

#if !WINDOWS_UWP && !NET_STANDARD
        #region ICloneable Members

        public override object Clone()
        {
            var ret = base.Clone() as ScriptGate;
            if (TagList != null)
            {
                var tagxml = TagList.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.TagList = tagxml.FromXml<OPCUAEntityReferenceList>();
            }

            return ret;
        }

        #endregion
#endif

        #region Abstracts

        public override void Init(IDocument doc) 
        {
            base.Init(doc);

            oldText = Text;
            engine = new Jint.Engine(cfg =>
            {
                // cfg.Strict(true); // Good practive for javascript
                cfg.AllowClr(); // Access to .net
                // cfg.LimitRecursion(16); // Help stop broken scripts taking down application
                cfg.CatchClrExceptions(ex => ex is Exception);

                if (SecTimeout > 0)
                    cfg.TimeoutInterval(TimeSpan.FromSeconds(SecTimeout));
                if (LimitRecursion >= 0)
                    cfg.LimitRecursion(LimitRecursion);
            });

            servers = new Servers(doc);
            engine.SetValue("Servers", servers);
        }

        public override void Terminate() 
        {
            base.Terminate();
            Text = oldText;
            if (servers != null)
            {
                servers.Dispose();
                servers = null;
            }
            _runtimeTagList = null;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public override bool CanDropTag() { return true; }

        public override bool DroppingTag(OPCUAEntityReference tag, String title)
        {
            var list = new OPCUAEntityReferenceList();
            if (TagList != null)
                list.AddRange(TagList);
            list.Add(tag);
            TagList = list;
            Text = title;
            return true;
        }
#endif
        public override bool ReadData()
        {
            bool ret = base.ReadData();
            if (ret)
            {
                if (TagList != null)
                {
                    foreach (var tag in TagList)
                    {
                        if (tag.MonitoredItemViewModel != null &&
                            tag.MonitoredItemViewModel.DataValue != null)
                        {
                            if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(tag.MonitoredItemViewModel.DataValue.StatusCode))
                                ret = true;
                            else
                                return false;
                        }
                    }
                }
                else if (_runtimeTagList != null)
                {
                    foreach (var tag in _runtimeTagList)
                    {
                        if (tag.MonitoredItemViewModel != null &&
                            tag.MonitoredItemViewModel.DataValue != null)
                        {
                            if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(tag.MonitoredItemViewModel.DataValue.StatusCode))
                                ret = true;
                            else
                                return false;
                        }
                    }
                }
            }

            return ret;
        }

        public override bool Execute(WireData[] links)
        {
            foreach (var data in links)
            {
                var resultString = Regex.Match(data.ToPort, @"\d+").Value;
                engine.SetValue(String.Format("Input{0}", Int32.Parse(resultString) - 1), data.Value);
            }
            if (TagList != null)
            {
                foreach (var tag in TagList)
                {
                    if (tag.MonitoredItemViewModel != null)
                        engine.SetValue(tag.HumanReadableNoProject.Replace('\\', '_').Replace(':', '_'), 
                            tag.MonitoredItemViewModel);
                }
            }
            else if (_runtimeTagList != null)
            {
                foreach (var tag in _runtimeTagList)
                {
                    if (tag.MonitoredItemViewModel != null)
                        engine.SetValue(tag.HumanReadableNoProject.Replace('\\', '_').Replace(':', '_'), 
                            tag.MonitoredItemViewModel);
                }
            }

            try
            {
                return engine.Execute(ScriptCode).GetCompletionValue().AsBoolean();
            }
            catch(Exception ex)
            {
                Text = ex.Message;
                throw ex;
            }
        }

        public override List<OPCUAEntityReference> GetTagList()
        {
            var ret = base.GetTagList();
            if (TagList != null)
                ret.AddRange(TagList);
            if (_runtimeTagList != null)
                ret.AddRange(_runtimeTagList);
            return ret;
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override void UpdateTagList(List<OPCUAEntityReference> list)
        {
            if (list == null)
                return;
            if (list.Count >= 1)
            {
                var listxml = list.ToXml();
                TagList = listxml.FromXml<OPCUAEntityReferenceList>();
                _runtimeTagList = listxml.FromXml<OPCUAEntityReferenceList>();
            }
        }
#endif

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Script";
#else
                return Properties.Settings.Default.ScriptGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD

        public override void OnDblClick() 
        {
            base.OnDblClick();

            var control = new JavaScriptEditor.JavaScriptEditor()
            {
                Width = 400,
                Height = 400,
                Text = ScriptCode
            };
            var Dialog = new GeneralDialogContent(control)
            {
                Title = String.Format(Properties.Resources.ScriptCode, Name)
                // Owner = this.FindParent<Window>(),
                // HelpLink = "ScreenSelector"
            };

            var code = ScriptCode;
            Dialog.Closing += (o, e) =>
            {
                code = control.Text;
            };

            if (Dialog.ShowDialog() == true)
                ScriptCode = code;

        }

        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/ScriptLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new ScriptGate() { Key = "Script", Text = "Script", GateDataTemplate = "Script", Category = "Script" });
            ret.Add(new ScriptGate() { Key = "Script", Text = "Script", GateDataTemplate = "ScriptFour", Category = "ScriptFour" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.ScriptCategory; }
#endif
        #endregion

        #region Properties

        String _ScriptCode;
        public String ScriptCode
        {
            get { return _ScriptCode; }
            set
            {
                var old = _ScriptCode;
                if (old != value)
                {
                    _ScriptCode = value;
                    RaisePropertyChanged("ScriptCode", old, value);
                }
            }
        }

        int _SecTimeout;
        public int SecTimeout
        {
            get { return _SecTimeout; }
            set
            {
                var old = _SecTimeout;
                if (old != value)
                {
                    _SecTimeout = value;
                    RaisePropertyChanged("SecTimeout", old, value);
                }
            }
        }

        int _LimitRecursion = 100;
        public int LimitRecursion
        {
            get { return _LimitRecursion; }
            set
            {
                var old = _LimitRecursion;
                if (old != value)
                {
                    _LimitRecursion = value;
                    RaisePropertyChanged("LimitRecursion", old, value);
                }
            }
        }

        OPCUAEntityReferenceList _runtimeTagList;
        OPCUAEntityReferenceList _TagList = new OPCUAEntityReferenceList();
        public OPCUAEntityReferenceList TagList
        {
            get { return _TagList; }
            set
            {
                var old = _TagList;
                if (old != value)
                {
                    _TagList = value;
                    RaisePropertyChanged("TagList", old, value);
                }
            }
        }

        #endregion

        #region Overrides

#if !WINDOWS_UWP && !NET_STANDARD
        // support standard reading/writing via Linq for XML
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("ScriptCode", this.ScriptCode, ""));
            e.Add(XHelper.Attribute("SecTimeout", this.SecTimeout, 0));
            e.Add(XHelper.Attribute("LimitRecursion", this.LimitRecursion, 100));
            if (this.TagList != null)
                e.Add(XHelper.Attribute("TagList", this.TagList.ToXml(), ""));

            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.ScriptCode = XHelper.Read("ScriptCode", e, "");
            this.SecTimeout = XHelper.Read("SecTimeout", e, 0);
            this.LimitRecursion = XHelper.Read("LimitRecursion", e, 100);
            var tagxml = XHelper.Read("TagList", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                TagList = tagxml.FromXml<OPCUAEntityReferenceList>();
        }
        #endregion
    }
}
