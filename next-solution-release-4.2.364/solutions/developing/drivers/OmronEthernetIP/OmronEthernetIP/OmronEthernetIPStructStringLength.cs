using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UFInterfaces.Editors;

namespace OmronEthernetIP
{
    public class OmronEthernetIPStructStringLength
    {
        public static char STRUCT_FIELD_SEPARATOR = '.';
        public static char FIELD_SEPARATOR = ';';
        public static string STRING_SIZE_SEPARATOR = ":";

        public class ProMemberView
        {
            public string Name { set; get; }
            public string Path { set; get; }
            public string FullName { set; get; }            
            public List<ProMemberView> Members { set; get; }
            public UFUAModel.DataType DataType { set; get; }
            public UFUAModel.ModelType ModelType { set; get; }
            public uint StringLength { set; get; }

            private void InitBase()
            {
                Name = string.Empty;
                Path = string.Empty;
                Members = new List<ProMemberView>();
                ModelType = UFUAModel.ModelType.Variable;
                DataType = UFUAModel.DataType.Boolean;
                StringLength = 0;
            }

            public ProMemberView()
            {
                InitBase();
            }

            public ProMemberView(string name, UFUAModel.ModelType modelType)
            {
                InitBase();
                Name = name;
                ModelType = modelType;
            }

            public ProMemberView(string name, string path, uint stringLength, UFUAModel.ModelType modelType) {
                
                InitBase();
                Name = name;
                Path = path;
                if (string.IsNullOrEmpty(Path))
                    FullName= Name;
                else
                    FullName = string.Format("{0}{1}{2}", Path, STRUCT_FIELD_SEPARATOR, Name);             
                Members = new List<ProMemberView>();
                StringLength = stringLength;
                if (StringLength == 0)
                    StringLength = OmronEthernetIPProtocol.MAX_STRING_LENGTH;
                ModelType = modelType;
                if (modelType != UFUAModel.ModelType.ObjectType)
                    DataType = UFUAModel.DataType.String;
            }
        }

        #region Constructors

        private void InitBase()
        {
            MemberView = null;
            membersView = new Dictionary<string, ProMemberView>();            
            parsingError = false;
            defaultStringLength = 0;
        }

        public OmronEthernetIPStructStringLength()
        {
            InitBase();
        }

        public void Parse(string structStringLength)
        {
            InitBase();
            membersView = ParseStructStringLength(structStringLength, out parsingError);
        }

        public void Parse(IDynamicSettingsEditing thisTag, string structStringLength)
        {
            InitBase();
            // first import prototype struct
            ImportPrototypeMembers(null, thisTag, ref MemberView);
            // then merge with actual configuration (from dinamic's link)
            membersView = MergeStructStringLengthWithPrototypeMembers(structStringLength);
        }

        #endregion

        #region Methods
        
        public static bool IsStringLengthInRange(uint stringLength)
        {
            return (stringLength > 0 && stringLength <= OmronEthernetIPProtocol.MAX_STRING_LENGTH);
        }
       
        private Dictionary<string, ProMemberView> ParseStructStringLength(string structStringLength, out bool error)
        {
            error = false;
            Dictionary<string, ProMemberView> members = new Dictionary<string, ProMemberView>();

            if (!string.IsNullOrEmpty(structStringLength))
            {
                // default max string size for all struct's member
                if (structStringLength.Substring(0, 1) == STRING_SIZE_SEPARATOR)
                {
                    if (uint.TryParse(structStringLength.Substring(1, structStringLength.Length - 1), out uint Dummy))
                    {
                        // for valid common string lenght
                        if (IsStringLengthInRange(Dummy))
                        {
                            defaultStringLength = Dummy;

                            // retrive list of all members of template and set to same size
                            members = GetMembersAsAList(MemberView);
                            System.Threading.Tasks.Parallel.ForEach(members.Values, m =>
                            {
                                m.StringLength = defaultStringLength;
                            });
                        }
                        else
                            error = true;
                    }
                    else
                    {
                        error = true;
                    }
                }
                else
                {
                    var structStringLengthMembers = structStringLength.Trim().Split(FIELD_SEPARATOR);
                    foreach (string member in structStringLengthMembers)
                    {
                        if (!string.IsNullOrEmpty(member))
                        {
                            var memberData = member.Split(char.Parse(STRING_SIZE_SEPARATOR));
                            if (memberData.Length == 2)
                            {
                                if (!string.IsNullOrEmpty(memberData[0]) && uint.TryParse(memberData[1], out uint dummy))
                                {
                                    if (!IsStringLengthInRange(dummy))
                                        dummy = OmronEthernetIPProtocol.MAX_STRING_LENGTH;

                                    members[memberData[0].ToLower()] = new ProMemberView(memberData[0], string.Empty, dummy, UFUAModel.ModelType.Variable);
                                }
                                else
                                    error = true;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }
            }
            
            // null or invalid parameters
            if (defaultStringLength == 0 && members.Count == 0)
                defaultStringLength = OmronEthernetIPProtocol.MAX_STRING_LENGTH;

            return members;
        }

        private void ImportPrototypeMembers(string ancestorPrototypeName, IDynamicSettingsEditing tag, ref ProMemberView memberView)
        {
            if (memberView == null)
            {
                memberView = new ProMemberView();
                memberView.Name = tag.Name;
                memberView.ModelType = UFUAModel.ModelType.ObjectType;
            }

            foreach (var member in tag.Members)
            {                
                if (member.DataType == (uint)UFUAModel.DataType.String && member.ArrayDimension == 0)
                {
                    ProMemberView newMemberView = new ProMemberView(member.Name, memberView.FullName, OmronEthernetIPProtocol.MAX_STRING_LENGTH,UFUAModel.ModelType.Variable);
                    memberView.Members.Add(newMemberView);
                }
                else if (member.IsObjectType)   // nested struct's element
                {
                    ProMemberView newMemberView = new ProMemberView(member.Name, memberView.FullName, 0, UFUAModel.ModelType.ObjectType);
                    
                    ImportPrototypeMembers(member.Name, member, ref newMemberView);

                    // if no "valid" members was addedr , don't add
                    if (newMemberView.Members.Count > 0)
                        memberView.Members.Add(newMemberView);
                }
            }           
        }       

        private Dictionary<string, ProMemberView> MergeStructStringLengthWithPrototypeMembers(string structStringLength)
        {
            Dictionary<string, ProMemberView> parsedMembers = ParseStructStringLength(structStringLength, out bool parsingError);
                        
            // merge parsed members with prototype members
            foreach (var member in parsedMembers.Values)
            {
                ProMemberView structMember = GetMemberView(member.Name);
                if (structMember != null)
                    structMember.StringLength = member.StringLength;
            }

            return GetMembersAsAList(MemberView);
        }

        private ProMemberView getMemberView(string memberName, ProMemberView memberView, ref bool found)
        {
            ProMemberView foundMember = null;

            foreach (var member in memberView.Members)
            {
                switch (member.ModelType)
                {
                    case UFUAModel.ModelType.Variable:
                        if (member.FullName == memberName)
                        {
                            foundMember = member;
                            found = true;
                            break;
                        }
                        break;
                    case UFUAModel.ModelType.ObjectType:
                        foundMember = getMemberView(memberName, member, ref found);
                        break;
                }

                if (found)
                    break;
            }

            return foundMember;
        }

        private ProMemberView GetMemberView(string memberName)
        {
            bool found = false;

            return getMemberView(memberName, MemberView, ref found);
        }

        public ProMemberView GetMember(string memberName)
        {
            if (membersView.ContainsKey(memberName.ToLower()))
                return membersView[memberName.ToLower()];
            else
                return null;
        }

        public bool HasMember(string memberName)
        {
            return (membersView.ContainsKey(memberName.ToLower()));
        }

        public bool HasMembers()
        {
            return (membersView.Count>0);
        }

        private void getMembersAsAList(ProMemberView memberView, ref Dictionary<string, ProMemberView> resultMembers)
        {
            if (memberView != null)
            {
                foreach (var member in memberView.Members)
                {
                    switch (member.ModelType)
                    {
                        case UFUAModel.ModelType.Variable:
                            resultMembers[member.FullName.ToLower()] = member;
                            break;
                        case UFUAModel.ModelType.ObjectType:
                            getMembersAsAList(member, ref resultMembers);
                            break;
                    }
                }
            }
        }

        private Dictionary<string, ProMemberView> GetMembersAsAList(ProMemberView memberView)
        {
            Dictionary<string, ProMemberView> resultMembers = new Dictionary<string, ProMemberView>();

            getMembersAsAList(memberView, ref resultMembers);

            return resultMembers;
        }

        public string UnSplitToStructString(ProMemberView memberView)
        {
            Dictionary<string, ProMemberView> members = GetMembersAsAList(memberView);
            if (members.Count == 0)
                return string.Empty;

            OmronEthernetIPStructStringLength.ProMemberView Member0 = members.First().Value;
            int NrEqualSize = members.Values.Count(m => m.StringLength == Member0.StringLength);
            if (NrEqualSize == members.Count())
                return string.Format("{0}{1}", STRING_SIZE_SEPARATOR, Member0.StringLength);


            string Result = string.Empty;
            foreach (var member in members.Values)// members.Values.ToList())
                Result += string.Format("{0}{1}{2}{3}", member.FullName, STRING_SIZE_SEPARATOR, member.StringLength, FIELD_SEPARATOR);

            return Result;
        }

        public bool IsGlobalStringLength()
        {
            return (defaultStringLength>0);
        }
        #endregion

        #region Properties       

        private uint defaultStringLength;
        public uint DefaultStringLength
        {
            get
            {
                return defaultStringLength;
            }
            set
            {
                defaultStringLength = value;
            }
        }        

        public ProMemberView MemberView;

        private Dictionary <string, ProMemberView> membersView;

        private bool parsingError;

        /// <summary>
        /// Return if during string parsing some parameters are invalid : used during check dynamic link
        /// </summary>
        public bool ParsingError { get { return parsingError; } }

        #endregion
    }
}
