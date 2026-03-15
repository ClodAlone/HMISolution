using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevExpress.Xpo.Metadata;
using Opc.Ua;
using System.Runtime.Serialization;
using System.Xml;


namespace UFUAModel
{
    [DataContract()]
    public class TagEntityReference : IFormattable, ICloneable
    {
        #region Static Members
        // {452BF8DD-560B-41C2-B590-40F5AC4E4C05}
        static Guid reservedGuid = new Guid("{ 0x452bf8dd, 0x560b, 0x41c2, { 0xb5, 0x90, 0x40, 0xf5, 0xac, 0x4e, 0x4c, 0x5 } }");

        /// <summary>
        /// A read-only instance of the TagEntityReference structure whose value is all zeros.
        /// </summary>
        public static readonly TagEntityReference Empty = new TagEntityReference();
        public static readonly TagEntityReference Reserved = new TagEntityReference(reservedGuid, String.Empty, NodeId.Null);
        #endregion

        #region Constructors
        /// <summary>
        /// Private ctor for initializing an empty TagEntityReference
        /// </summary>
        TagEntityReference()
        {
            this.guid = Guid.Empty;
            this.name = string.Empty;
            this.nodeId = Opc.Ua.NodeId.Null;
        }

        /// <summary>
        /// Initializes a new instance of the TagEntityReference structure by using the specified tag
        /// </summary>
        public TagEntityReference(UFUAModel.UFUATag tag)
        {
            this.guid = tag.NodeId;
            this.name = tag.GetRelativeName();
            this.nodeId = Opc.Ua.NodeId.Null;
        }

        /// <summary>
        /// Initializes a new instance of the TagEntityReference structure by using the guid and name
        /// </summary>
        public TagEntityReference(Guid guid, String name)
        {
            this.guid = guid;
            this.name = name;
            this.nodeId = Opc.Ua.NodeId.Null;
        }

        /// <summary>
        /// Initializes a new instance of the TagEntityReference structure by using the nodeId and name
        /// </summary>
        public TagEntityReference(Guid guid, String name, NodeId nodeId)
        {
            this.guid = guid;
            this.name = name;
            this.nodeId = nodeId;
        }

        public TagEntityReference(Guid guid, String name, NodeId nodeId, string humanreadeable)
        {
            this.guid = guid;
            this.name = name;
            this.nodeId = nodeId;
            this.humanReadable = humanreadeable;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Get the NodeId used for initializing this instance of TagEntityReference.
        /// </summary>        
        public NodeId NodeId
        {
            get
            {
                return nodeId;
            }
        }
                
        /// <summary>
        /// Get the string Name used for initializing this instance of TagEntityReference.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }
                
        /// <summary>
        /// Get the Guid used for initializing this instance of TagEntityReference.
        /// </summary>
        public Guid Guid
        {
            get
            {
                return guid;
            }
        }
        
        String humanReadable;
        [DataMember]
        public String HumanReadable
        {
            get
            {
                return String.IsNullOrEmpty(humanReadable) ? string.Empty : humanReadable;
            }
            set
            {
                if (humanReadable == value)
                    return;
                humanReadable = value;
            }
        }
                
        public String HumanReadableNoProject
        {
            get
            {
                if (String.IsNullOrEmpty(humanReadable))
                    return String.Empty;
                var splitted = humanReadable.Split(new String[] { " (" }, StringSplitOptions.RemoveEmptyEntries);
                return splitted[0];
            }
        }
                
        public String StringRepresentation
        {
            get
            {
                string stringRef = "";
                NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                string oldChars = string.Format("{0}:", ns);
                if (!String.IsNullOrEmpty(Name))
                {
                    var tags = "Tags\\";
                    var s = Name.Replace(oldChars, "").Replace('/', '\\').Replace('&', '\\');
                    if (s.StartsWith(tags))
                        s = s.Substring(tags.Length);
                    stringRef = s;
                    var index = HumanReadableNoProject.IndexOf(':');
                    if (index != -1)
                    {
                        var find = HumanReadableNoProject.Substring(0, index);
                        stringRef = stringRef.Replace(String.Format("{0}\\", find), String.Format("{0}:", find));
                    }
                }
                return !String.IsNullOrEmpty(stringRef) ? stringRef : HumanReadableNoProject;
            }
        }
                
        public String StringRepresentationWithProject
        {
            get
            {
                if (String.IsNullOrEmpty(StringRepresentation) || String.IsNullOrEmpty(humanReadable))
                {
                    if (!String.IsNullOrEmpty(StringRepresentation))
                        return StringRepresentation;
                    return String.Empty;
                }
                var splitted = humanReadable.Split(new String[] { " (" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitted.Length == 0)
                    return String.Empty;
                return splitted.Length == 1 ? splitted[0] : String.Format("{0} ({1}", StringRepresentation, splitted[1]);
            }
        }

        #endregion

        #region Public Members
        /// <summary>
        /// Check if the TagEntityReference is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return guid == Guid.Empty;
        }

        /// <summary>
        /// Return the NodeId of the TagEntityReference structure
        /// </summary>
        /// <param name="namespaceindex"></param>
        /// <returns></returns>
        public NodeId ResolveNodeId(ushort namespaceindex)
        {
            if (nodeId == null && guid != Guid.Empty)
                return new NodeId(guid, namespaceindex);
            else if (nodeId.NamespaceIndex != namespaceindex && guid != Guid.Empty)
                return new NodeId(guid, namespaceindex);

            return nodeId;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Return a string rappresentation of the TagEntityReference
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!String.IsNullOrEmpty(name))
                return Regex.Replace(name, "[0-9]+:", "");
            else if (guid == Guid.Empty)
                return String.Empty;

            return NameOfGuid();
        }
        #endregion

        #region IFormattable Members

        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(format))
                return this.ToString();

            if (format.Contains("{0}") && format.Contains("{1}") && format.Contains("{2}") && format.Contains("{3}"))
                return String.Format(formatProvider, format, NameOfGuid(), name, nodeId, humanReadable);
            else if (format.Contains("{0}") && format.Contains("{1}") && format.Contains("{2}"))
                return String.Format(formatProvider, format, NameOfGuid(), name, nodeId);
            else if (format.Contains("{0}") && format.Contains("{1}"))
                return String.Format(formatProvider, format, NameOfGuid(), name);
            else if (format.Contains("{0}"))
                return String.Format(formatProvider, format, NameOfGuid());
            else
                return this.ToString();
        }

        String NameOfGuid()
        {
            if (guid == reservedGuid)
                return Properties.Resources.ReservedGuidName;

            return guid.ToString();
        }

        #endregion

        #region Private Members
        [DataMember]
        readonly NodeId nodeId;
        [DataMember]
        readonly string name;
        [DataMember]
        readonly Guid guid;
        #endregion

        #region ICloneable Members

        TagEntityReference(TagEntityReference template)
        {
            if (template == null)
                return;

            guid = template.Guid;
            name = template.Name;
            nodeId = template.NodeId;
            humanReadable = template.humanReadable;
        }

        public object Clone()
        {
            return new TagEntityReference(this);
        }

        #endregion
    }

    #region Converters for XPObject

    public class ConvertTagEntityReference : ValueConverter
    {
        public override object ConvertFromStorageType(object value)
        {
            var storage = value as String;
            if (storage == null)
                return TagEntityReference.Empty;

            return CreateTagEntityReference(storage);
        }

        public override object ConvertToStorageType(object value)
        {
            var entity = value as TagEntityReference;
            if (entity == null)
                return String.Empty;
            else if (entity.Guid == TagEntityReference.Reserved.Guid)
                return TagEntityReference.Reserved.Guid.ToString();

            return entity.ToString("{0}|{1}|{2}|{3}");
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }

        static TagEntityReference CreateTagEntityReference(string entity)
        {
            Guid guid = Guid.Empty;
            String name = String.Empty;
            NodeId nodeId = NodeId.Null;
            string humanreadeable = String.Empty;
            var values = entity.Split('|');
            if (values.Length > 0)
            {
                if (!Guid.TryParse(values[0], out guid) && guid != TagEntityReference.Reserved.Guid)
                    nodeId = NodeId.Parse(values[0]);
            }
            if (values.Length > 1)
            {
                name = values[1];
            }
            if (values.Length > 2)
            {
                nodeId = NodeId.Parse(values[2]);
            }
            if (values.Length > 3)
            {
                humanreadeable = values[3];
            }

            if (!NodeId.IsNull(nodeId))
                return new TagEntityReference(guid, name, nodeId, humanreadeable);
            else
                return new TagEntityReference(guid, name);
        }
    }

    #endregion
}
