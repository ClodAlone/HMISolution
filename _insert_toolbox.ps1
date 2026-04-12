$file = "C:\Users\cfior\source\repos\ServerEditorWeb\Components\Editor\ScreenToolboxPanel.razor"
$lines = [System.IO.File]::ReadAllLines($file, [System.Text.Encoding]::UTF8)
$newLines = [System.Collections.Generic.List[string]]::new($lines)
$insertIdx = 15
