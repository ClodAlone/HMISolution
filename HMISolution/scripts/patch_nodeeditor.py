file_path = "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Services\\NodeEditorService.cs"

with open(file_path, "rb") as f:
    content = f.read().decode("utf-8")

print(f"Read {len(content)} chars")

# 1. Add Events group to BuildTree, after batchGroup block
marker1 = "        proj.Children.Add(batchGroup);\r\n\r\n        var screenGroup"
if marker1 not in content:
    marker1 = "        proj.Children.Add(batchGroup);\n\n        var screenGroup"

if marker1 not in content:
    print("ERROR: marker1 not found")
    exit(1)

insert1 = """        proj.Children.Add(batchGroup);

        var eventGroup = new EventGroupNode() { Parent = proj };
        if (proj.Model.Events != null)
        {
            foreach (var evt in proj.Model.Events)
            {
                var eNode = new EventNode(evt) { Parent = eventGroup };
                eventGroup.Children.Add(eNode);
            }
        }
        proj.Children.Add(eventGroup);

        var screenGroup"""

content = content.replace(marker1, insert1, 1)
print("1. Added Events to BuildTree")

# 2. Add AddEvent method after AddBatch
marker2 = "    public void AddBatch()"
idx2 = content.find(marker2)
if idx2 < 0:
    print("ERROR: AddBatch not found")
    exit(1)

# Find the end of AddBatch method (closing brace after it)
# AddBatch has: if body with { }, method closing }
# Find 2 closing braces after the opening
brace_count = 0
i = idx2
started = False
while i < len(content):
    if content[i] == '{':
        brace_count += 1
        started = True
    elif content[i] == '}':
        brace_count -= 1
        if started and brace_count == 0:
            break
    i += 1

end_of_add_batch = i + 1
# Skip to end of line
while end_of_add_batch < len(content) and content[end_of_add_batch] in ('\r', '\n'):
    end_of_add_batch += 1

add_event_method = """
    public void AddEvent()
    {
        if (SelectedItem is EventGroupNode parent)
        {
            var newEvent = new EventConfig
            {
                Name = "New Event",
                Enabled = true
            };
            var newNode = new EventNode(newEvent) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

"""

content = content[:end_of_add_batch] + add_event_method + content[end_of_add_batch:]
print("2. Added AddEvent method")

# 3. Add RebuildModelStructure case for EventGroupNode after BatchGroupNode case
marker3 = "            else if (root is BatchGroupNode batchGrpNode && _rootModel != null)"
idx3 = content.find(marker3)
if idx3 < 0:
    print("ERROR: BatchGroupNode rebuild not found")
    exit(1)

# Find end of BatchGroupNode block
brace_count = 0
i = idx3
started = False
while i < len(content):
    if content[i] == '{':
        brace_count += 1
        started = True
    elif content[i] == '}':
        brace_count -= 1
        if started and brace_count == 0:
            break
    i += 1

end_of_batch_rebuild = i + 1
# Skip newline
while end_of_batch_rebuild < len(content) and content[end_of_batch_rebuild] in ('\r', '\n'):
    end_of_batch_rebuild += 1

event_rebuild = """            else if (root is EventGroupNode eventGrpNode && _rootModel != null)
            {
                _rootModel.Events.Clear();
                foreach (var child in eventGrpNode.Children)
                {
                    if (child is EventNode eNode)
                    {
                        eNode.SyncName();
                        _rootModel.Events.Add(eNode.Event);
                    }
                }
            }
"""

content = content[:end_of_batch_rebuild] + event_rebuild + content[end_of_batch_rebuild:]
print("3. Added EventGroupNode to RebuildModelStructure")

with open(file_path, "w", encoding="utf-8", newline="") as f:
    f.write(content)

print(f"Written {len(content)} chars - Done")
