using OPCUAViewModel;
using ScreenSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using UFInterfaces;
using Utilities;

namespace DynamicTagAwareHelper
{
    public class TypeHelper : IDisposable
    {
        public bool ChecktypeDefinition(String SessionName, ScreenDocument Document, IEntityReference er, bool bDesign, OPCUAXMLEntityReference Tag, String relative, String absolute, PropertyChangedEventHandler propertyChangedEventHandler, PropertyChangedEventHandler monitoredPropertyChangedEventHandler, ref OPCUAEntityReference tag, MonitoredItemViewModel monitored)
        {
            OPCUAEntityReference _relative = relative.FromXml<OPCUAEntityReference>();
            OPCUAEntityReference _absolute = absolute.FromXml<OPCUAEntityReference>();
            bool ret = false;
            try
            {
                if (Tag != null && Tag.TagReference != null)
                {
                    if (bDesign)
                        ret = relative == Tag.TagReferenceXml;
                    else if(_absolute != null)
                    {
                        if (relative == Tag.TagReferenceXml)
                        {
                            TerminateExecution(er, propertyChangedEventHandler, monitoredPropertyChangedEventHandler, tag, monitored);
                            if(_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                            {
                                _relative.Merge(_absolute);
                                tag = _relative;
                            }
                            else
                                tag = _absolute;

                            PrepareExecution(SessionName, Document, er, propertyChangedEventHandler, tag);
                            ret = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return ret;
        }
        public void TerminateExecution(IEntityReference er, PropertyChangedEventHandler propertyChangedEventHandler, PropertyChangedEventHandler monitoredPropertyChangedEventHandler, OPCUAEntityReference tag, MonitoredItemViewModel monitored)
        {
            if (monitored != null)
                monitored.PropertyChanged -= monitoredPropertyChangedEventHandler;

            if (tag != null)
            {
                tag.PropertyChanged -= propertyChangedEventHandler;

                if (tag.IsValid)
                    tag.SetInUse(er, false);
            }
        }
        public void PrepareExecution(String SessionName, ScreenDocument Document, IEntityReference er, PropertyChangedEventHandler propertyChangedEventHandler, OPCUAEntityReference tag)
        {
            if (tag != null && tag.IsValid)
            {
                tag.PropertyChanged -= propertyChangedEventHandler;
                if (bDisposed)
                    return;

                tag.PropertyChanged += propertyChangedEventHandler;

                //FogBugz 11581
                if (Document != null && !string.IsNullOrEmpty(Document.SessionString))
                    tag.Resolve(Document.SessionString, Document);
                else
                    tag.Resolve(SessionName, Document);

                tag.SetInUse(er, true);
            }
        }

        public string UpdateTag(OPCUAEntityReference tag, Dictionary<string, string> map)
        {
            if (tag != null && tag.IsValid)
            {
                try
                {
                    var _old = tag;
                    var oldname = GetReferenceName(_old);
                    foreach (var key in map.Keys)
                    {
                        var entity = key.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                        String name = GetReferenceName(entity);
                        if (String.IsNullOrEmpty(name))
                            continue;
                        if (name == oldname)
                            return map[key];
                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
        public string UpdateTag(string tag, Dictionary<string, string> map)
        {
            try
            {
                var _old = tag.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                var oldname = _old.RelativePath; //GetReferenceName(_old);
                foreach (var key in map.Keys)
                {
                    var entity = key.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                    String name = entity.RelativePath;// GetReferenceName(entity);
                    if (String.IsNullOrEmpty(name))
                        continue;
                    if (name == oldname)
                        return map[key];
                }
            }
            catch (Exception)
            {
            }

            return tag;
        }
        String GetReferenceName(OPCUAEntityReference reference)
        {
            if (reference.ResolvedNodeId != null)
                return reference.ResolvedNodeId.ToString();
            else if (reference.StartingAddress != null)
                return String.Format("{0}-{1}", reference.StartingAddress, reference.RelativePath);

            return reference.HumanReadable; 
        }

        public Dictionary<string, string> PreserveTagsFromMap(Dictionary<String, String> dynamicMap, Dictionary<String, String> map)
        {
            Dictionary<string, string> updatemap = new Dictionary<string, string>();

            foreach (var c in map)
            {
                if (dynamicMap.ContainsKey(c.Key))
                {
                    var elementString = dynamicMap[c.Key];
                    var sourceString = string.Empty;
                    using (var reader = new System.IO.StringReader(c.Value))
                    {
                        using (var textReader = new XmlTextReader(reader))
                        {
                            var obj = System.Windows.Markup.XamlReader.Load(textReader);
                            try
                            {
                                if (obj is OPCUAXMLEntityReference)
                                {
                                    sourceString = (obj as OPCUAXMLEntityReference).TagReferenceXml;
                                }
                                else if (obj is string && !String.IsNullOrEmpty(obj as string))
                                {
                                    if ((obj as string).FromXml<OPCUAEntityReference>() != null)
                                        sourceString = (obj as string);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    if (!updatemap.ContainsKey(elementString) && !string.IsNullOrEmpty(sourceString))
                        updatemap.Add(elementString, sourceString);
                }
            }

            return updatemap;
        }

        bool bDisposed;
        public void Dispose()
        {
            bDisposed = true;
        }
    }
}
