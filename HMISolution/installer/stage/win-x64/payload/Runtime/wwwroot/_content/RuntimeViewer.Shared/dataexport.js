// Data Export — triggers a file download in the browser from a string.
// Loaded as an ES module via JS.InvokeAsync("import", ...).
export function downloadText(filename, contentType, text) {
    const blob = new Blob([text], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}
