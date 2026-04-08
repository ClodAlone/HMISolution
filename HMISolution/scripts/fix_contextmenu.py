path = r"C:\Users\cfior\source\repos\ServerEditorWeb\Components\Editor\TreeViewContextMenu.razor"

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

print(f"Read {len(content)} chars")

# Fix the corrupted ScreenGroupNode block - the EventGroupNode was inserted inside it
old_broken = (
    '        @if (Node is ScreenGroupNode or (ResourceFolderNode { ResourceKind: "Screen" }\r\n'
    '        @if (Node is EventGroupNode)\r\n'
    '        {\r\n'
    '            <div class="dock-context-item" @onclick="AddEvent">\U0001f514 Add Event</div>\r\n'
    '        }))\r\n'
    '        {\r\n'
    '            <div class="dock-context-item" @onclick="AddFolder">\U0001f4c1 Add Folder</div>\r\n'
    '            <div class="dock-context-item" @onclick="AddScreen">\U0001f5bc\ufe0f Add Screen</div>\r\n'
    '        }'
)

new_fixed = (
    '        @if (Node is EventGroupNode)\r\n'
    '        {\r\n'
    '            <div class="dock-context-item" @onclick="AddEvent">\U0001f514 Add Event</div>\r\n'
    '        }\r\n'
    '        @if (Node is ScreenGroupNode or (ResourceFolderNode { ResourceKind: "Screen" }))\r\n'
    '        {\r\n'
    '            <div class="dock-context-item" @onclick="AddFolder">\U0001f4c1 Add Folder</div>\r\n'
    '            <div class="dock-context-item" @onclick="AddScreen">\U0001f5bc\ufe0f Add Screen</div>\r\n'
    '        }'
)

if old_broken in content:
    content = content.replace(old_broken, new_fixed, 1)
    print("Fixed EventGroupNode / ScreenGroupNode block")
    with open(path, 'w', encoding='utf-8', newline='') as f:
        f.write(content)
    print(f"Written {len(content)} chars - Done")
else:
    print("ERROR: broken block not found exactly")
    # Let's find what's there
    idx = content.find("ScreenGroupNode or (ResourceFolderNode")
    if idx >= 0:
        snippet = content[idx:idx+400]
        print(f"Found at {idx}: {repr(snippet[:300])}")
    else:
        print("ScreenGroupNode anchor not found at all")
