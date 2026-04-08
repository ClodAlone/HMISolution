import os

path = os.path.join(r"C:\Users\cfior\source\repos", "ServerEditorWeb", "Components", "Editor", "NodeProperties.razor")

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

# Find exact scheduler navigate screen section 
idx = content.find('cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal"')
positions = []
start = 0
while True:
    pos = content.find('cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal"', start)
    if pos == -1:
        break
    positions.append(pos)
    start = pos + 1

print(f"Found {len(positions)} occurrences of screen action check")
for i, pos in enumerate(positions):
    # Show context
    line_start = content.rfind('\n', 0, pos) + 1
    line_end = content.find('\n', pos)
    # Show a wider range
    snippet_start = max(0, pos - 20)
    snippet_end = min(len(content), pos + 200)
    snippet = content[snippet_start:snippet_end]
    print(f"\n--- Occurrence {i+1} at position {pos} ---")
    print(repr(snippet[:150]))
