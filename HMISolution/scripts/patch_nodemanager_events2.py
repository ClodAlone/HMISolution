path = r"C:\Users\cfior\source\repos\Server\SimpleFileServerNodeManager.cs"

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

original_len = len(content)
print(f"Read {original_len} chars")

changes = 0

# 2. Add Events initialization block after Batch block (use exact indentation from file)
old_batch = """                  _batchManager.Initialize(nodeModel.BatchSequences);
                  }
                              // ─── Diagnostics OPC UA node"""

new_batch = """                  _batchManager.Initialize(nodeModel.BatchSequences);
                  }

                  // ─── Events (condition \u2192 command) ───
                  if (nodeModel.Events != null && nodeModel.Events.Count > 0)
                  {
                      CreateEventVariables(nodeModel.Events, references);
                      _eventManager = new EventManager(this);
                      _eventManager.Initialize(nodeModel.Events);
                  }
                              // ─── Diagnostics OPC UA node"""

if "CreateEventVariables" not in content:
    if old_batch in content:
        content = content.replace(old_batch, new_batch, 1)
        changes += 1
        print("2. Added Events initialization block")
    else:
        print("2. ERROR: anchor not found. Searching...")
        # Try to find the exact sequence
        idx = content.find("_batchManager.Initialize(nodeModel.BatchSequences);")
        if idx >= 0:
            # Find the next occurrence of "Diagnostics"
            diag_idx = content.find("Diagnostics OPC UA node", idx)
            if diag_idx >= 0:
                print(f"   Found batch at {idx}, diagnostics at {diag_idx}")
                # Get the exact text between them
                between = content[idx:diag_idx]
                print(f"   Between text: {repr(between[:200])}")
else:
    print("2. SKIP init block (already exists)")

# 3. Add CreateEventVariables method after CreateAssetVar method
# Find the exact end of CreateAssetVar
marker = "_variables[path] = variable;\n        }\n"
# Find the second occurrence (after CreateAssetVar, not CreateAssetVariables)
pos = content.find("private void CreateAssetVar<T>")
if pos >= 0 and "CreateEventVariables" not in content:
    end_pos = content.find(marker, pos)
    if end_pos >= 0:
        insert_at = end_pos + len(marker)
        event_methods = """
        private void CreateEventVariables(List<SharedModels.EventConfig> events, IList<IReference>? references)
        {
            var rootFolder = new FolderState(null)
            {
                NodeId = new NodeId("_Events", _namespaceIndex),
                BrowseName = new QualifiedName("_Events", _namespaceIndex),
                DisplayName = new LocalizedText("Events"),
                TypeDefinitionId = ObjectTypeIds.FolderType
            };
            if (references != null)
                rootFolder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            AddPredefinedNode(SystemContext, rootFolder);

            foreach (var evt in events)
            {
                if (!evt.Enabled) continue;

                var folder = new FolderState(rootFolder)
                {
                    NodeId = new NodeId($"_Events.{evt.Name}", _namespaceIndex),
                    BrowseName = new QualifiedName(evt.Name, _namespaceIndex),
                    DisplayName = new LocalizedText(evt.Name),
                    TypeDefinitionId = ObjectTypeIds.FolderType
                };
                rootFolder.AddChild(folder);
                AddPredefinedNode(SystemContext, folder);

                var prefix = $"_Events.{evt.Name}";
                CreateEventVar<bool>(folder, prefix, "Active", false);
                CreateEventVar<DateTime>(folder, prefix, "LastFired", DateTime.MinValue);
            }
        }

        private void CreateEventVar<T>(FolderState parent, string prefix, string name, T defaultValue)
        {
            var path = $"{prefix}.{name}";
            var variable = new BaseDataVariableState<T>(parent)
            {
                NodeId = new NodeId(path, _namespaceIndex),
                BrowseName = new QualifiedName(name, _namespaceIndex),
                DisplayName = new LocalizedText(name),
                DataType = Opc.Ua.TypeInfo.GetDataTypeId(typeof(T)),
                ValueRank = ValueRanks.Scalar, Value = defaultValue,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                Timestamp = DateTime.UtcNow, StatusCode = StatusCodes.Good
            };
            parent.AddChild(variable);
            AddPredefinedNode(SystemContext, variable);
            _variables[path] = variable;
        }
"""
        content = content[:insert_at] + event_methods + content[insert_at:]
        changes += 1
        print("3. Added CreateEventVariables + CreateEventVar methods")
    else:
        print("3. ERROR: could not find end of CreateAssetVar")
elif "CreateEventVariables" in content:
    print("3. SKIP CreateEventVariables (already exists)")
else:
    print("3. ERROR: CreateAssetVar not found")

with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"\nWritten {len(content)} chars ({changes} changes applied) - Done")
