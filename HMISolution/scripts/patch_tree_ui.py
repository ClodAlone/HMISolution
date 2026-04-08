import sys

def patch_file(path, patches):
    with open(path, "rb") as f:
        content = f.read().decode("utf-8")
    print(f"  Read {len(content)} chars from {path}")
    for desc, old, new in patches:
        if old in content:
            content = content.replace(old, new, 1)
            print(f"  OK: {desc}")
        else:
            print(f"  ERROR: {desc} - marker not found")
            # Try to find partial match for debugging
            short = old[:60].replace('\r','\\r').replace('\n','\\n')
            print(f"    Looking for: {short}...")
            return False
    with open(path, "w", encoding="utf-8", newline="") as f:
        f.write(content)
    print(f"  Written {len(content)} chars")
    return True

# ─── 1. TreeViewContextMenu.razor ───
print("\n=== TreeViewContextMenu.razor ===")
ok = patch_file(
    "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Editor\\TreeViewContextMenu.razor",
    [
        # Add context menu item for EventGroupNode
        (
            "Add EventGroup context menu entry after BatchGroup",
            '        @if (Node is BatchGroupNode)\n        {\n            <div class="dock-context-item" @onclick="AddBatch">\U0001F504 Add Batch Sequence</div>\n        }',
            '        @if (Node is BatchGroupNode)\n        {\n            <div class="dock-context-item" @onclick="AddBatch">\U0001F504 Add Batch Sequence</div>\n        }\n        @if (Node is EventGroupNode)\n        {\n            <div class="dock-context-item" @onclick="AddEvent">\U0001F514 Add Event</div>\n        }'
        ),
        # Add EventNode to copy list
        (
            "Add EventNode to copyable node types",
            "or AssetNode or BatchNode or ScreenNode",
            "or AssetNode or BatchNode or EventNode or ScreenNode"
        ),
        # Add parameter
        (
            "Add OnAddEvent parameter",
            "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }",
            "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }\n    [Parameter] public EventCallback<TreeNode> OnAddEvent { get; set; }"
        ),
        # Add handler method
        (
            "Add AddEvent handler method",
            "    private void AddBatch() { OnAddBatch.InvokeAsync(Node!); Close(); }",
            "    private void AddBatch() { OnAddBatch.InvokeAsync(Node!); Close(); }\n    private void AddEvent() { OnAddEvent.InvokeAsync(Node!); Close(); }"
        ),
    ]
)
if not ok:
    sys.exit(1)

# ─── 2. TreeView.razor ───
print("\n=== TreeView.razor ===")
ok = patch_file(
    "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Editor\\TreeView.razor",
    [
        # Add OnAddEvent to TreeViewItem usage in template
        (
            "Add OnAddEvent to TreeViewItem in template",
            'OnAddCalculated="OnAddCalculated" OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch" OnAddScreen="OnAddScreen"',
            'OnAddCalculated="OnAddCalculated" OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch" OnAddEvent="OnAddEvent" OnAddScreen="OnAddScreen"'
        ),
        # Add parameter declaration
        (
            "Add OnAddEvent parameter declaration",
            "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }",
            "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }\n    [Parameter] public EventCallback<TreeNode> OnAddEvent { get; set; }"
        ),
    ]
)
if not ok:
    sys.exit(1)

# ─── 3. TreeViewItem.razor ───
print("\n=== TreeViewItem.razor ===")
ok = patch_file(
    "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Editor\\TreeViewItem.razor",
    [
        # Add OnAddEvent to context menu instance
        (
            "Add OnAddEvent to TreeViewContextMenu instance",
            '            OnAddBatch="OnAddBatch"',
            '            OnAddBatch="OnAddBatch"\n            OnAddEvent="OnAddEvent"'
        ),
        # Add OnAddEvent to recursive TreeViewItem child
        (
            "Add OnAddEvent to recursive TreeViewItem child",
            'OnAddCalculated="OnAddCalculated" OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch" OnAddScreen="OnAddScreen"',
            'OnAddCalculated="OnAddCalculated" OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch" OnAddEvent="OnAddEvent" OnAddScreen="OnAddScreen"'
        ),
        # Add parameter declaration
        (
            "Add OnAddEvent parameter declaration",
            "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }",
            "    [Parameter] public EventCallback<TreeNode> OnAddBatch { get; set; }\n    [Parameter] public EventCallback<TreeNode> OnAddEvent { get; set; }"
        ),
    ]
)
if not ok:
    sys.exit(1)

# ─── 4. Home.razor ───
print("\n=== Home.razor ===")
ok = patch_file(
    "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Pages\\Home.razor",
    [
        # Add OnAddEvent to TreeView usage
        (
            "Add OnAddEvent to TreeView usage",
            'OnAddCalculated="OnAddCalculated" OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch"',
            'OnAddCalculated="OnAddCalculated" OnAddAsset="OnAddAsset" OnAddBatch="OnAddBatch" OnAddEvent="OnAddEvent"'
        ),
        # Add OnAddEvent handler method
        (
            "Add OnAddEvent handler method",
            'private void OnAddBatch(TreeNode node) { Editor.SelectNode(node); Editor.AddBatch(); }',
            'private void OnAddBatch(TreeNode node) { Editor.SelectNode(node); Editor.AddBatch(); }\nprivate void OnAddEvent(TreeNode node) { Editor.SelectNode(node); Editor.AddEvent(); }'
        ),
    ]
)
if not ok:
    sys.exit(1)

# ─── 5. ClipboardService.cs ───
print("\n=== ClipboardService.cs ===")
ok = patch_file(
    "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Services\\ClipboardService.cs",
    [
        (
            "Add Event to CanPasteInto",
            '        "Report" => target is ReportGroupNode,',
            '        "Report" => target is ReportGroupNode,\n        "Event" => target is EventGroupNode,'
        ),
    ]
)
if not ok:
    sys.exit(1)

print("\n=== All patches applied successfully ===")
