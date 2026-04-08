import sys

def patch_file(path, patches):
    """Apply patches. Each patch is (desc, old_marker, new_text).
    The old_marker and new_text use | as a line separator to avoid newline issues."""
    with open(path, "rb") as f:
        content = f.read().decode("utf-8")

    # Detect line ending
    if "\r\n" in content:
        nl = "\r\n"
    else:
        nl = "\n"

    nl_name = 'CRLF' if nl == '\r\n' else 'LF'
    print(f"  Read {len(content)} chars (nl={nl_name})")

    for desc, old, new in patches:
        if old in content:
            content = content.replace(old, new, 1)
            print(f"  OK: {desc}")
        else:
            print(f"  ERROR: {desc}")
            short = old[:80]
            print(f"    Searching: {repr(short)}...")
            return False

    with open(path, "w", encoding="utf-8", newline="") as f:
        f.write(content)
    print(f"  Written {len(content)} chars")
    return True

def read_file(path):
    with open(path, "rb") as f:
        return f.read().decode("utf-8")

def write_file(path, content):
    with open(path, "w", encoding="utf-8", newline="") as f:
        f.write(content)

# ─── 1. TreeViewContextMenu.razor ───
print("\n=== TreeViewContextMenu.razor ===")
path = "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Editor\\TreeViewContextMenu.razor"
content = read_file(path)
nl = "\r\n" if "\r\n" in content else "\n"
print(f"  Read {len(content)} chars")

# 1a. Add context menu after BatchGroupNode block
old1 = '        @if (Node is BatchGroupNode)' + nl + '        {' + nl + '            <div class="dock-context-item" @onclick="AddBatch">'
idx = content.find(old1)
if idx < 0:
    print("  ERROR: BatchGroupNode context menu not found")
    sys.exit(1)
# Find the closing brace of this @if block
end_idx = content.index("}", content.index("}", idx + len(old1)) + 1) + 1
# Insert after
insert = nl + '        @if (Node is EventGroupNode)' + nl + '        {' + nl + '            <div class="dock-context-item" @onclick="AddEvent">\U0001F514 Add Event</div>' + nl + '        }'
content = content[:end_idx] + insert + content[end_idx:]
print("  OK: Added EventGroupNode context menu")

# 1b. Add EventNode to copy list
content = content.replace("or AssetNode or BatchNode or ScreenNode", "or AssetNode or BatchNode or EventNode or ScreenNode", 1)
print("  OK: Added EventNode to copy list")

# 1c. Add OnAddEvent parameter
content = content.replace(
    "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }",
    "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }" + nl + "    [Parameter] public EventCallback<TreeNode> OnAddEvent { get; set; }",
    1
)
print("  OK: Added OnAddEvent parameter")

# 1d. Add AddEvent handler
content = content.replace(
    "    private void AddBatch() { OnAddBatch.InvokeAsync(Node!); Close(); }",
    "    private void AddBatch() { OnAddBatch.InvokeAsync(Node!); Close(); }" + nl + "    private void AddEvent() { OnAddEvent.InvokeAsync(Node!); Close(); }",
    1
)
print("  OK: Added AddEvent handler")

write_file(path, content)
print(f"  Written {len(content)} chars")

# ─── 2. TreeView.razor ───
print("\n=== TreeView.razor ===")
path = "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Editor\\TreeView.razor"
content = read_file(path)
nl = "\r\n" if "\r\n" in content else "\n"
print(f"  Read {len(content)} chars")

content = content.replace(
    "OnAddAsset=\"OnAddAsset\" OnAddBatch=\"OnAddBatch\" OnAddScreen=",
    "OnAddAsset=\"OnAddAsset\" OnAddBatch=\"OnAddBatch\" OnAddEvent=\"OnAddEvent\" OnAddScreen=",
    1
)
content = content.replace(
    "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }",
    "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }" + nl + "    [Parameter] public EventCallback<TreeNode> OnAddEvent { get; set; }",
    1
)
write_file(path, content)
print(f"  Written {len(content)} chars")

# ─── 3. TreeViewItem.razor ───
print("\n=== TreeViewItem.razor ===")
path = "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Editor\\TreeViewItem.razor"
content = read_file(path)
nl = "\r\n" if "\r\n" in content else "\n"
print(f"  Read {len(content)} chars")

# Context menu instance
content = content.replace(
    '            OnAddBatch="OnAddBatch"',
    '            OnAddBatch="OnAddBatch"' + nl + '            OnAddEvent="OnAddEvent"',
    1
)
# Recursive child
content = content.replace(
    'OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch" OnAddScreen=',
    'OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch" OnAddEvent="OnAddEvent" OnAddScreen=',
    1
)
# Parameter
content = content.replace(
    "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }",
    "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }" + nl + "    [Parameter] public EventCallback<TreeNode> OnAddEvent { get; set; }",
    1
)
write_file(path, content)
print(f"  Written {len(content)} chars")

# ─── 4. Home.razor ───
print("\n=== Home.razor ===")
path = "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Pages\\Home.razor"
content = read_file(path)
nl = "\r\n" if "\r\n" in content else "\n"
print(f"  Read {len(content)} chars")

content = content.replace(
    'OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch"',
    'OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch" OnAddEvent="OnAddEvent"',
    1
)
content = content.replace(
    'private void OnAddBatch(TreeNode node) { Editor.SelectNode(node); Editor.AddBatch(); }',
    'private void OnAddBatch(TreeNode node) { Editor.SelectNode(node); Editor.AddBatch(); }' + nl + 'private void OnAddEvent(TreeNode node) { Editor.SelectNode(node); Editor.AddEvent(); }',
    1
)
write_file(path, content)
print(f"  Written {len(content)} chars")

# ─── 5. ClipboardService.cs ───
print("\n=== ClipboardService.cs ===")
path = "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Services\\ClipboardService.cs"
content = read_file(path)
nl = "\r\n" if "\r\n" in content else "\n"
print(f"  Read {len(content)} chars")

content = content.replace(
    '        "Report" => target is ReportGroupNode,',
    '        "Report" => target is ReportGroupNode,' + nl + '        "Event" => target is EventGroupNode,',
    1
)
write_file(path, content)
print(f"  Written {len(content)} chars")

print("\n=== All patches applied ===")
