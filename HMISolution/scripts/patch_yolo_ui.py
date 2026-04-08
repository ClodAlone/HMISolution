"""Patch ScreenSymbolProperties.razor to add YoloEnableVariable picker."""

path = r"C:\Users\cfior\source\repos\ServerEditorWeb\Components\Editor\ScreenSymbolProperties.razor"
with open(path, "r", encoding="utf-8") as f:
    content = f.read()

# Insert YoloEnableVariable picker right after the @if (EnableYolo) { block opens,
# before the ONNX Model Path section
old = '''            @if (_selectedSymbol.Camera.EnableYolo)
            {
                <div class="mb-2">
                    <label class="form-label mb-0 small">ONNX Model Path</label>'''

new = '''            @if (_selectedSymbol.Camera.EnableYolo)
            {
                <div class="mb-2">
                    <label class="form-label mb-0 small">Enable/Disable Variable</label>
                    <VariablePathPicker Value="@_selectedSymbol.Camera.YoloEnableVariable"
                                        ValueChanged='v => { _selectedSymbol.Camera!.YoloEnableVariable = v ?? string.Empty; }'
                                        OnChanged="OnChanged"
                                        Placeholder="(optional — Boolean variable to enable/disable at runtime)" />
                    <div class="form-text" style="font-size: 9px;">
                        Bind a Boolean server variable to dynamically enable/disable YOLO detection at runtime.
                        Leave empty to always run detection when enabled above.
                    </div>
                </div>

                <div class="mb-2">
                    <label class="form-label mb-0 small">ONNX Model Path</label>'''

assert old in content, "EnableYolo block not found"
content = content.replace(old, new, 1)
print("Added YoloEnableVariable picker to ScreenSymbolProperties.razor")

with open(path, "w", encoding="utf-8") as f:
    f.write(content)
print("   -> ScreenSymbolProperties.razor saved")
