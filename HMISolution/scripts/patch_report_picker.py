"""Patch NodeProperties.razor: replace GenerateReport text inputs with ReportPicker."""
import re

path = r"C:\Users\cfior\source\repos\ServerEditorWeb\Components\Editor\NodeProperties.razor"
with open(path, "rb") as f:
    data = f.read()

# --- Occurrence 1 & 2: CRLF (scheduler sections) ---
old_crlf = (
    b'<input type="text" class="form-control form-control-sm" style="font-size: 10px;"'
    b' placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />'
)
new_crlf = (
    b'<ReportPicker Value="@cmd.TargetReport"\r\n'
    b'                                  ValueChanged=\'v => { cmd.TargetReport = v ?? string.Empty; }\'\r\n'
    b'                                  OnChanged="OnChanged"\r\n'
    b'                                  Placeholder="(select report)" />'
)

# --- Occurrence 3 & 4: LF (event sections) ---
old_lf_input = old_crlf  # same bytes for the input tag itself
new_lf = (
    b'<ReportPicker Value="@cmd.TargetReport"\n'
    b'                              ValueChanged=\'v => { cmd.TargetReport = v ?? string.Empty; }\'\n'
    b'                              OnChanged="OnChanged"\n'
    b'                              Placeholder="(select report)" />'
)

count = data.count(old_crlf)
print(f"Found {count} occurrences of the GenerateReport input tag")

# We need to differentiate by context. The CRLF ones have \r\n after the tag,
# and the LF ones have \n after the tag.
# Strategy: find each occurrence, check what follows, and replace accordingly.

result = bytearray()
pos = 0
replaced = 0
while True:
    idx = data.find(old_crlf, pos)
    if idx == -1:
        result.extend(data[pos:])
        break
    result.extend(data[pos:idx])
    after = data[idx + len(old_crlf) : idx + len(old_crlf) + 2]
    if after == b'\r\n':
        result.extend(new_crlf)
        print(f"  Replaced occurrence {replaced+1} at byte {idx} (CRLF context)")
    else:
        result.extend(new_lf)
        print(f"  Replaced occurrence {replaced+1} at byte {idx} (LF context)")
    pos = idx + len(old_crlf)
    replaced += 1

print(f"\nTotal replaced: {replaced}")

with open(path, "wb") as f:
    f.write(bytes(result))

print("Done — NodeProperties.razor patched.")
