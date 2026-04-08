import os

path = r"C:\Users\cfior\source\repos\Server\SimpleFileServerNodeManager.cs"

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

original_len = len(content)
print(f"Read {original_len} chars")

changes = 0

# 1. Add _eventManager field after _batchManager
old_field = "private BatchSequenceManager? _batchManager;"
new_field = "private BatchSequenceManager? _batchManager;\n        private EventManager? _eventManager;"
if old_field in content and "_eventManager" not in content:
    content = content.replace(old_field, new_field, 1)
    changes += 1
    print("1. Added _eventManager field")
else:
    print("1. SKIP field (already exists or anchor not found)")

# 2. Add Events initialization block after Batch block
old_batch_block = """                  // ─── Batch / Sequence Manager ───
                  if (nodeModel.BatchSequences != null && nodeModel.BatchSequences.Count > 0)
                  {
                      CreateBatchVariables(nodeModel.BatchSequences, references);
                      _batchManager = new BatchSequenceManager(this);
                      _batchManager.Initialize(nodeModel.BatchSequences);
                  }"""

new_batch_block = old_batch_block + """

                  // ─── Events (condition → command) ───
                  if (nodeModel.Events != null && nodeModel.Events.Count > 0)
                  {
                      CreateEventVariables(nodeModel.Events, references);
                      _eventManager = new EventManager(this);
                      _eventManager.Initialize(nodeModel.Events);
                  }"""

if old_batch_block in content and "CreateEventVariables" not in content:
    content = content.replace(old_batch_block, new_batch_block, 1)
    changes += 1
    print("2. Added Events initialization block")
else:
    print("2. SKIP init block (already exists or anchor not found)")

# 3. Add CreateEventVariables method after CreateAssetVar method
old_create_asset_var_end = """        private void CreateAssetVar<T>(FolderState parent, string prefix, string name, T defaultValue)
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
        }"""

new_create_event_vars = old_create_asset_var_end + """

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
        }"""

if old_create_asset_var_end in content and "CreateEventVariables" not in content:
    content = content.replace(old_create_asset_var_end, new_create_event_vars, 1)
    changes += 1
    print("3. Added CreateEventVariables + CreateEventVar methods")
else:
    print("3. SKIP CreateEventVariables (already exists or anchor not found)")

# 4. Add _eventManager?.Dispose() in Dispose method
old_dispose = "_batchManager?.Dispose();"
new_dispose = "_batchManager?.Dispose();\n                _eventManager?.Dispose();"
if old_dispose in content and "_eventManager?.Dispose()" not in content:
    content = content.replace(old_dispose, new_dispose, 1)
    changes += 1
    print("4. Added _eventManager?.Dispose()")
else:
    print("4. SKIP dispose (already exists or anchor not found)")

with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"\nWritten {len(content)} chars ({changes} changes applied) - Done")
