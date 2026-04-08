import sys

path = r"C:\Users\cfior\source\repos\ServerEditorWeb\Components\Editor\NodeProperties.razor"
with open(path, "rb") as f:
    data = f.read()

needle = b'placeholder="Report name"'
pos = 0
for i in range(4):
    idx = data.find(needle, pos)
    if idx == -1:
        print(f"Only found {i} occurrences")
        break
    snippet = data[idx - 80 : idx + 150]
    print(f"Occurrence {i+1} at byte {idx}:")
    print(repr(snippet))
    print()
    pos = idx + len(needle)
