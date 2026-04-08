path = r"C:\Users\cfior\source\repos\Server\SimpleFileServerNodeManager.cs"

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

original_len = len(content)
print(f"Read {original_len} chars")

nl = '\r\n'

# Check if the method definition already exists (not just the call)
method_def = "private void CreateEventVariables(List<SharedModels.EventConfig>"
if method_def in content:
    print("CreateEventVariables method already exists. Nothing to do.")
else:
    # Find CreateAssetVar<T> method definition, then its end
    asset_var_pos = content.find("private void CreateAssetVar<T>")
    if asset_var_pos >= 0:
        # Find the closing brace of this method - look for the pattern "_variables[path] = variable;" 
        # followed by "        }"
        search_from = asset_var_pos
        # Find the _variables[path] = variable; line after CreateAssetVar
        vars_line = content.find("_variables[path] = variable;", search_from)
        if vars_line >= 0:
            # Find the closing brace line after it
            next_brace = content.find(f"{nl}        }}{nl}", vars_line)
            if next_brace >= 0:
                insert_at = next_brace + len(f"{nl}        }}")
                print(f"Found insert point at position {insert_at}")

                event_methods = (
                    f"{nl}"
                    f"{nl}"
                    f"        private void CreateEventVariables(List<SharedModels.EventConfig> events, IList<IReference>? references){nl}"
                    f"        {{{nl}"
                    f"            var rootFolder = new FolderState(null){nl}"
                    f"            {{{nl}"
                    f"                NodeId = new NodeId(\"_Events\", _namespaceIndex),{nl}"
                    f"                BrowseName = new QualifiedName(\"_Events\", _namespaceIndex),{nl}"
                    f"                DisplayName = new LocalizedText(\"Events\"),{nl}"
                    f"                TypeDefinitionId = ObjectTypeIds.FolderType{nl}"
                    f"            }};{nl}"
                    f"            if (references != null){nl}"
                    f"                rootFolder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);{nl}"
                    f"            AddPredefinedNode(SystemContext, rootFolder);{nl}"
                    f"{nl}"
                    f"            foreach (var evt in events){nl}"
                    f"            {{{nl}"
                    f"                if (!evt.Enabled) continue;{nl}"
                    f"{nl}"
                    f"                var folder = new FolderState(rootFolder){nl}"
                    f"                {{{nl}"
                    f"                    NodeId = new NodeId($\"_Events.{{evt.Name}}\", _namespaceIndex),{nl}"
                    f"                    BrowseName = new QualifiedName(evt.Name, _namespaceIndex),{nl}"
                    f"                    DisplayName = new LocalizedText(evt.Name),{nl}"
                    f"                    TypeDefinitionId = ObjectTypeIds.FolderType{nl}"
                    f"                }};{nl}"
                    f"                rootFolder.AddChild(folder);{nl}"
                    f"                AddPredefinedNode(SystemContext, folder);{nl}"
                    f"{nl}"
                    f"                var prefix = $\"_Events.{{evt.Name}}\";{nl}"
                    f"                CreateEventVar<bool>(folder, prefix, \"Active\", false);{nl}"
                    f"                CreateEventVar<DateTime>(folder, prefix, \"LastFired\", DateTime.MinValue);{nl}"
                    f"            }}{nl}"
                    f"        }}{nl}"
                    f"{nl}"
                    f"        private void CreateEventVar<T>(FolderState parent, string prefix, string name, T defaultValue){nl}"
                    f"        {{{nl}"
                    f"            var path = $\"{{prefix}}.{{name}}\";{nl}"
                    f"            var variable = new BaseDataVariableState<T>(parent){nl}"
                    f"            {{{nl}"
                    f"                NodeId = new NodeId(path, _namespaceIndex),{nl}"
                    f"                BrowseName = new QualifiedName(name, _namespaceIndex),{nl}"
                    f"                DisplayName = new LocalizedText(name),{nl}"
                    f"                DataType = Opc.Ua.TypeInfo.GetDataTypeId(typeof(T)),{nl}"
                    f"                ValueRank = ValueRanks.Scalar, Value = defaultValue,{nl}"
                    f"                AccessLevel = AccessLevels.CurrentRead,{nl}"
                    f"                UserAccessLevel = AccessLevels.CurrentRead,{nl}"
                    f"                Timestamp = DateTime.UtcNow, StatusCode = StatusCodes.Good{nl}"
                    f"            }};{nl}"
                    f"            parent.AddChild(variable);{nl}"
                    f"            AddPredefinedNode(SystemContext, variable);{nl}"
                    f"            _variables[path] = variable;{nl}"
                    f"        }}"
                )
                content = content[:insert_at] + event_methods + content[insert_at:]
                print("Added CreateEventVariables + CreateEventVar methods")

                with open(path, 'w', encoding='utf-8', newline='') as f:
                    f.write(content)
                print(f"Written {len(content)} chars - Done")
            else:
                print("ERROR: could not find closing brace after _variables[path]")
        else:
            print("ERROR: could not find _variables[path] after CreateAssetVar")
    else:
        print("ERROR: CreateAssetVar<T> method not found")
