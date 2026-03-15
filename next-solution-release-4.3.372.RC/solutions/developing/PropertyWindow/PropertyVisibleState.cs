using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PropertyControl
{
    public struct PropertyVisibleState
    {
        #region Public Members
        public readonly static PropertyVisibleState UnsetValue = new PropertyVisibleState()
        {
            IsAlwaysHiden = false,
            IsShowInEsyMode = false,
            IsAdvancedProperty = false,
            Priority = 0,
            ProjectTypeToHide = new List<string>(),
        };

        public bool IsAlwaysHiden;
        public bool IsShowInEsyMode;
        public bool IsAdvancedProperty;
        public int Priority;
        public List<string> ProjectTypeToHide;

        #endregion

        #region Constants

        const string rootNode = "VisibleState";
        const string startElement = "property";

        public const string HideAlwaysAttribute = "HideAlways";
        public const string ShowEeasyModeAttribute = "ShowEeasyMode";
        public const string AdvancedPropertyAttribute = "AdvancedProperty";
        public const string PriorityAttribute = "Priority";
        public const string ProjectTypeToHideAttribute = "ProjectTypeToHide";

        #endregion

        #region Overloads

        public override bool Equals(object obj)
        {
            // The given object to compare to can't be null
            if (obj == null) return false;
            // If objects are different types, they can't be equal.
            if (this.GetType() != obj.GetType()) return false;
            // If objects are same type, return true if all of their fields match
            // Since System.Object defines no fields, the fields match
            return this.IsShowInEsyMode == ((PropertyVisibleState)obj).IsShowInEsyMode && 
                    this.IsAlwaysHiden == ((PropertyVisibleState)obj).IsAlwaysHiden &&
                    this.IsAdvancedProperty == ((PropertyVisibleState)obj).IsAdvancedProperty && 
                    this.Priority == ((PropertyVisibleState)obj).Priority &&
                    this.ProjectTypeToHide == ((PropertyVisibleState)obj).ProjectTypeToHide;
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetHashCode()"/> method from IntelliSense.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Static Methods
#if !WINDOWS_UWP
        static IEnumerable<dynamic> GetExpandoStateFromXml(string xmlString)
        {
            /* can be used...
            var expandolist = GetExpandoFromXml("http://phejndorf.wordpress.com/feed/", "item");
            expandolist.ToList().ForEach(element =>
            {
                var dictionary = element as IDictionary<string, object>;
                dictionary.ToList().ForEach(d => Console.WriteLine("{0}: {1}", d.Key, d.Value));
            });
            */
            var expandoFromXml = new List<dynamic>();

            //var doc = XDocument.Load(file);
            XDocument doc = new XDocument();
            doc = XDocument.Parse(xmlString);

            foreach (var element in doc.Descendants(rootNode))
            {
                PropertyVisibleState state = UnsetValue;
                dynamic expandoObject = new ExpandoObject();
                var dictionary = expandoObject as IDictionary<string, object>;
                foreach (var child in element.Descendants())
                {
                    if (child.Name.Namespace == "")
                    {
                        lock (dictionary)
                        {
                            if (child.Attribute(HideAlwaysAttribute) == null || !Boolean.TryParse(child.Attribute(HideAlwaysAttribute).Value, out state.IsAlwaysHiden))
                                state.IsAlwaysHiden = false;
                            if (child.Attribute(ShowEeasyModeAttribute) == null || !Boolean.TryParse(child.Attribute(ShowEeasyModeAttribute).Value, out state.IsShowInEsyMode))
                                state.IsShowInEsyMode = false;
                            if (child.Attribute(AdvancedPropertyAttribute) == null || !Boolean.TryParse(child.Attribute(AdvancedPropertyAttribute).Value, out state.IsAdvancedProperty))
                                state.IsAdvancedProperty = false;
                            if (child.Attribute(PriorityAttribute) == null || !int.TryParse(child.Attribute(PriorityAttribute).Value, out state.Priority))
                                state.Priority = 0;
                            if (child.Attribute(ProjectTypeToHideAttribute) != null)
                                state.ProjectTypeToHide = child.Attribute(ProjectTypeToHideAttribute).Value.Split('|').ToList();

                            dictionary[child.Value] = state;
                        }
                    }
                }
                yield return expandoObject;
            }
        }

        public static void LoadPropertyVisibleStatesFromXml(String filepath, IDictionary<String, PropertyVisibleState> mapItems)
        {
            if (File.Exists(filepath))
            {
                mapItems.Clear();

                try
                {
                    var keyexpandolist = PropertyVisibleState.GetExpandoStateFromXml(File.ReadAllText(filepath));
                    if (keyexpandolist.Count() != 0)
                    {
                        keyexpandolist.ToList().ForEach(e =>
                        {
                            var regkeydictionary = e as IDictionary<string, object>;
                            regkeydictionary.ToList().ForEach(r =>
                            {
                                PropertyVisibleState state = UnsetValue;
                                state.IsAlwaysHiden = ((PropertyVisibleState)r.Value).IsAlwaysHiden;
                                state.IsShowInEsyMode = ((PropertyVisibleState)r.Value).IsShowInEsyMode;
                                state.IsAdvancedProperty = ((PropertyVisibleState)r.Value).IsAdvancedProperty;
                                state.Priority = ((PropertyVisibleState)r.Value).Priority;
                                state.ProjectTypeToHide = ((PropertyVisibleState)r.Value).ProjectTypeToHide;
                                mapItems.Add(r.Key, (PropertyVisibleState)state);
                            });
                        });
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        public static void WritePropertyVisibleStatesToXml(String filepath, IDictionary<String, PropertyVisibleState> mapItems)
        {
            if (File.Exists(filepath))
            {
                try
                {
                    File.Delete(filepath);
                }
                catch (Exception ex)
                {

                }
            }

            if (!File.Exists(filepath))
            {
                var settings = new System.Xml.XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };

                using (var writer = System.Xml.XmlWriter.Create(filepath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(rootNode);

                    foreach (var key in mapItems.Keys)
                    {
                        if (mapItems[key] != UnsetValue)
                        {
                            writer.WriteStartElement(startElement);
                            writer.WriteAttributeString(HideAlwaysAttribute, mapItems[key].IsAlwaysHiden ? "True" : "False");
                            writer.WriteAttributeString(ShowEeasyModeAttribute, mapItems[key].IsShowInEsyMode ? "True" : "False");
                            writer.WriteAttributeString(AdvancedPropertyAttribute, mapItems[key].IsAdvancedProperty ? "True" : "False");
                            string projToHideStr = String.Join("|", mapItems[key].ProjectTypeToHide);
                            writer.WriteAttributeString(ProjectTypeToHideAttribute, projToHideStr);
                            writer.WriteAttributeString(PriorityAttribute, mapItems[key].Priority.ToString());
                            writer.WriteString(key);
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }
#endif
        #endregion

        #region Operators

        public static bool operator ==(PropertyVisibleState obj1, PropertyVisibleState obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(PropertyVisibleState obj1, PropertyVisibleState obj2)
        {
            return !obj1.Equals(obj2);
        }

        #endregion
    }
}
