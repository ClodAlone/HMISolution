"""
Patch script for 5 issues:
1. Add TargetLocale property to SymbolCommand (SharedModels/NodeModels.cs)
2. Create ScreenPicker.razor component  
3. Create LocalePicker.razor component
4. Fix scheduler command screen list (use ScreenPicker) + add locale picker for ChangeLanguage
5. Add screen/language commands to Event command editors + use pickers
6. Fix ScreenSymbolProperties to use LocalePicker for ChangeLanguage
7. Desktop save-before-close prompt
"""
import os

# We handle each file separately and report success for each

BASE = r"C:\Users\cfior\source\repos"

def read_file(rel):
    path = os.path.join(BASE, rel)
    with open(path, 'rb') as f:
        return f.read().decode('utf-8'), path

def write_file(path, content):
    with open(path, 'w', encoding='utf-8', newline='') as f:
        f.write(content)

results = []

# =========================================================
# 1. SharedModels/NodeModels.cs — Add TargetLocale property
# =========================================================
try:
    content, path = read_file(r"SharedModels\NodeModels.cs")
    if "TargetLocale" not in content:
        anchor = '    /// <summary>Target report name (for GenerateReport).</summary>\r\n        public string TargetReport { get; set; } = "";'
        replacement = anchor + '\r\n\r\n        /// <summary>Target locale/language code (for ChangeLanguage). E.g. "en", "de", "it".</summary>\r\n        public string TargetLocale { get; set; } = "";'
        if anchor in content:
            content = content.replace(anchor, replacement, 1)
            write_file(path, content)
            results.append("1. NodeModels.cs: Added TargetLocale property to SymbolCommand")
        else:
            results.append("1. NodeModels.cs: SKIP - anchor not found")
    else:
        results.append("1. NodeModels.cs: SKIP - TargetLocale already exists")
except Exception as e:
    results.append(f"1. NodeModels.cs: ERROR - {e}")

for r in results:
    print(r)
print("Phase 1 done")
