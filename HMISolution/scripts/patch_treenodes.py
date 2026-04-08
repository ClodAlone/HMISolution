file_path = "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Models\\TreeNodeModels.cs"

with open(file_path, "rb") as f:
    content = f.read().decode("utf-8")

print(f"Read {len(content)} chars")

marker = "    public void SyncName() => Batch.Name = Name;\r\n}"
idx = content.find(marker)
if idx < 0:
    # Try with just \n
    marker = "    public void SyncName() => Batch.Name = Name;\n}"
    idx = content.find(marker)

if idx < 0:
    print("ERROR: could not find BatchNode end marker")
    # Debug: find the line
    for i, line in enumerate(content.split("\n")):
        if "Batch.Name = Name" in line:
            print(f"  Found at line {i}: {repr(line)}")
    exit(1)

insert_pos = idx + len(marker)
new_code = """

public class EventGroupNode : TreeNode
{
    public override string TypeName => "EventGroup";
    public override string Icon => "\u26A1";
    public EventGroupNode() { Name = "Events"; }
}

public class EventNode : TreeNode
{
    public override string TypeName => "Event";
    public override string Icon => "\U0001F514";
    public EventConfig Event { get; }
    public EventNode(EventConfig evt) { Event = evt; Name = evt.Name; }
    public void SyncName() => Event.Name = Name;
}"""

content = content[:insert_pos] + new_code + content[insert_pos:]
print("Added EventGroupNode and EventNode")

with open(file_path, "w", encoding="utf-8", newline="") as f:
    f.write(content)

print(f"Written {len(content)} chars - Done")
