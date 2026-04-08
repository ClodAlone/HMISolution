"""Move YoloEnableVariable picker outside the @if (EnableYolo) block so it's always visible."""

path = r"C:\Users\cfior\source\repos\ServerEditorWeb\Components\Editor\ScreenSymbolProperties.razor"
with open(path, "r", encoding="utf-8") as f:
    content = f.read()

# Remove the YoloEnableVariable block from inside the @if (EnableYolo) block
old_inside = '''            @if (_selectedSymbol.Camera.EnableYolo)
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

new_inside = '''            <div class="mb-2">
                <label class="form-label mb-0 small">Enable/Disable Variable</label>
                <VariablePathPicker Value="@_selectedSymbol.Camera.YoloEnableVariable"
                                    ValueChanged='v => { _selectedSymbol.Camera!.YoloEnableVariable = v ?? string.Empty; }'
                                    OnChanged="OnChanged"
                                    Placeholder="(optional \u2014 Boolean variable to enable/disable at runtime)" />
                <div class="form-text" style="font-size: 9px;">
                    Bind a Boolean server variable to dynamically enable/disable YOLO detection at runtime.
                    When this variable is <code>true</code>, detection runs; when <code>false</code>, it is skipped.
                    Leave empty to always follow the checkbox above.
                </div>
            </div>

            @if (_selectedSymbol.Camera.EnableYolo)
            {
                <div class="mb-2">
                    <label class="form-label mb-0 small">ONNX Model Path</label>'''

assert old_inside in content, "YoloEnableVariable block not found in expected location"
content = content.replace(old_inside, new_inside, 1)
print("Moved YoloEnableVariable picker outside @if (EnableYolo) block")

with open(path, "w", encoding="utf-8") as f:
    f.write(content)
print("   -> ScreenSymbolProperties.razor saved")
