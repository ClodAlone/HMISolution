path = r"C:\Users\cfior\source\repos\Server\SimpleFileServerNodeManager.cs"

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

original_len = len(content)
print(f"Read {original_len} chars")

# Detect line ending
if '\r\n' in content:
    nl = '\r\n'
    print("Line ending: CRLF")
else:
    nl = '\n'
    print("Line ending: LF")

changes = 0

# 2. Add Events initialization block after Batch block
batch_anchor = f"_batchManager.Initialize(nodeModel.BatchSequences);{nl}                  }}{nl}                              // ─── Diagnostics OPC UA node"

if "CreateEventVariables" not in content:
    if batch_anchor in content:
        replacement = (
            f"_batchManager.Initialize(nodeModel.BatchSequences);{nl}"
            f"                  }}{nl}"
            f"{nl}"
            f"                  // ─── Events (condition \u2192 command) ───{nl}"
            f"                  if (nodeModel.Events != null && nodeModel.Events.Count > 0){nl}"
            f"                  {{{nl}"
            f"                      CreateEventVariables(nodeModel.Events, references);{nl}"
            f"                      _eventManager = new EventManager(this);{nl}"
            f"                      _eventManager.Initialize(nodeModel.Events);{nl}"
            f"                  }}{nl}"
            f"                              // ─── Diagnostics OPC UA node"
        )
        content = content.replace(batch_anchor, replacement, 1)
        changes += 1
        print("2. Added Events initialization block")
    else:
        print("2. ERROR: batch anchor not found")
        # Debug
        idx = content.find("_batchManager.Initialize(nodeModel.BatchSequences);")
        if idx >= 0:
            snippet = content[idx:idx+200]
            print(f"   Found at {idx}: {repr(snippet[:150])}")
else:
    print("2. SKIP init block (already exists)")

# 3. Add CreateEventVariables method after CreateAssetVar
asset_var_end = f"_variables[path] = variable;{nl}        }}"

if "CreateEventVariables" not in content:
    # Find CreateAssetVar<T> method, then its closing _variables line
    asset_var_pos = content.find("private void CreateAssetVar<T>")
    if asset_var_pos >= 0:
        end_pos = content.find(asset_var_end, asset_var_pos)
        if end_pos >= 0:
            insert_at = end_pos + len(asset_var_end)
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
            changes += 1
            print("3. Added CreateEventVariables + CreateEventVar methods")
        else:
            print("3. ERROR: could not find end of CreateAssetVar")
            # Debug
            snippet = content[asset_var_pos:asset_var_pos+800]
            print(f"   Snippet: {repr(snippet[-200:])}")
    else:
        print("3. ERROR: CreateAssetVar not found")
else:
    print("3. SKIP CreateEventVariables (already exists)")

with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"\nWritten {len(content)} chars ({changes} changes applied) - Done")
